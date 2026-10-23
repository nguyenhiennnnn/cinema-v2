import { Camera, CameraOff, AlertTriangle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useQrCameraScanner } from "../hooks/use-qr-camera-scanner";

interface CheckinCameraPanelProps {
  active: boolean;
  onToggle: () => void;
  onDetect: (value: string) => void;
}

export const CheckinCameraPanel = ({ active, onToggle, onDetect }: CheckinCameraPanelProps) => {
  const { videoRef, isSupported, error } = useQrCameraScanner({ active, onDetect });

  return (
    <div className="flex flex-col gap-3">
      <div className="relative aspect-square w-full overflow-hidden border border-border/60 bg-black">
        {active ? (
          <video ref={videoRef} muted playsInline className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-muted-foreground">
            <CameraOff className="size-8" />
          </div>
        )}
        {active && (
          <div className="pointer-events-none absolute inset-6 border-2 border-primary/70">
            <div className="absolute left-0 top-0 h-6 w-6 rounded-tl-lg border-l-4 border-t-4 border-accent" />
            <div className="absolute right-0 top-0 h-6 w-6 rounded-tr-lg border-r-4 border-t-4 border-accent" />
            <div className="absolute bottom-0 left-0 h-6 w-6 rounded-bl-lg border-b-4 border-l-4 border-accent" />
            <div className="absolute bottom-0 right-0 h-6 w-6 rounded-br-lg border-b-4 border-r-4 border-accent" />
            <div className="absolute inset-x-0 top-0 h-0.5 animate-pulse bg-primary shadow-[0_0_10px_2px]" />
          </div>
        )}
      </div>

      {error && (
        <div className="flex items-center gap-2 bg-destructive/10 px-3 py-2 text-xs text-destructive">
          <AlertTriangle className="size-3.5" />
          <span>{error}</span>
        </div>
      )}

      {!isSupported && !error && (
        <div className="flex items-center gap-2 bg-muted px-3 py-2 text-xs text-muted-foreground">
          <AlertTriangle className="size-3.5" />
          <span>Trình duyệt không hỗ trợ quét QR bằng camera</span>
        </div>
      )}

      <Button type="button" variant={active ? "outline" : "secondary"} onClick={onToggle}>
        {active ? <CameraOff className="size-4" /> : <Camera className="size-4" />}
        <span>{active ? "Tắt camera" : "Bật camera quét mã"}</span>
      </Button>
    </div>
  );
}