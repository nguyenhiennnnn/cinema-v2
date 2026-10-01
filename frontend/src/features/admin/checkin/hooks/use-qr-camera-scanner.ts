import { useCallback, useEffect, useRef, useState } from "react";
import jsQR from "jsqr";

interface BarcodeDetectorResult {
  rawValue: string;
}

interface BarcodeDetectorLike {
  detect: (source: CanvasImageSource) => Promise<BarcodeDetectorResult[]>;
}

declare global {
  interface Window {
    BarcodeDetector?: new (options: {
      formats: string[];
    }) => BarcodeDetectorLike;
  }
}

interface UseQrCameraScannerOptions {
  active: boolean;
  onDetect: (value: string) => void;
}

export function useQrCameraScanner({
  active,
  onDetect,
}: UseQrCameraScannerOptions) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const streamRef = useRef<MediaStream | null>(null);
  const rafRef = useRef<number>(0);
  const detectorRef = useRef<BarcodeDetectorLike | null>(null);
  const lastValueRef = useRef<string | null>(null);
  const lastTimeRef = useRef(0);
  const [isSupported] = useState(
    () =>
      typeof window !== "undefined" && !!navigator.mediaDevices?.getUserMedia,
  );
  const [error, setError] = useState<string | null>(null);

  const stopCamera = useCallback(() => {
    if (rafRef.current) cancelAnimationFrame(rafRef.current);
    streamRef.current?.getTracks().forEach((track) => track.stop());
    streamRef.current = null;
    if (videoRef.current) videoRef.current.srcObject = null;
  }, []);

  const detectFromCanvas = useCallback(() => {
    const video = videoRef.current;
    if (!video || video.readyState < video.HAVE_CURRENT_DATA) return null;
    if (!canvasRef.current)
      canvasRef.current = document.createElement("canvas");
    const canvas = canvasRef.current;
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext("2d");
    if (!ctx || canvas.width === 0 || canvas.height === 0) return null;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    const result = jsQR(imageData.data, imageData.width, imageData.height, {
      inversionAttempts: "dontInvert",
    });
    return result?.data || null;
  }, []);

  const scanFrame = useCallback(async () => {
    let value: string | null = null;
    try {
      if (detectorRef.current) {
        const results = await detectorRef.current.detect(videoRef.current!);
        value = results[0]?.rawValue || null;
      } else {
        value = detectFromCanvas();
      }
    } catch {
      value = null;
    }

    if (value) {
      const now = Date.now();
      if (value !== lastValueRef.current || now - lastTimeRef.current > 2500) {
        lastValueRef.current = value;
        lastTimeRef.current = now;
        onDetect(value);
      }
    }
    rafRef.current = requestAnimationFrame(scanFrame);
  }, [detectFromCanvas, onDetect]);

  const startCamera = useCallback(async () => {
    if (!isSupported) {
      setError(
        location.protocol === "https:" || location.hostname === "localhost"
          ? "Trình duyệt không hỗ trợ truy cập camera"
          : "Camera chỉ hoạt động trên kết nối HTTPS",
      );
      return;
    }
    setError(null);
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: "environment" },
      });
      streamRef.current = stream;
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        await videoRef.current.play();
      }
      detectorRef.current = window.BarcodeDetector
        ? new window.BarcodeDetector({ formats: ["qr_code"] })
        : null;
      rafRef.current = requestAnimationFrame(scanFrame);
    } catch (err) {
      const name = (err as { name?: string })?.name;
      if (name === "NotAllowedError") {
        setError(
          "Bạn đã từ chối quyền truy cập camera. Vui lòng cấp quyền trong trình duyệt.",
        );
      } else if (name === "NotFoundError") {
        setError("Không tìm thấy camera trên thiết bị này");
      } else {
        setError("Không thể truy cập camera");
      }
    }
  }, [isSupported, scanFrame]);

  useEffect(() => {
    if (active) {
      startCamera();
    } else {
      stopCamera();
    }
    return () => stopCamera();
  }, [active, startCamera, stopCamera]);

  return { videoRef, isSupported, error };
}
