import { useEffect, useRef } from "react";

const COLORS = [
  "oklch(0.4 0.15 23)",
  "oklch(0.6 0.086 74)",
  "oklch(0.5 0.11 152)",
  "oklch(0.55 0.1 200)",
  "oklch(0.5 0.11 350)",
];

interface Particle {
  x: number;
  y: number;
  vx: number;
  vy: number;
  rotation: number;
  vr: number;
  size: number;
  color: string;
  shape: "rect" | "circle";
}

/** Bắn pháo giấy toàn màn hình một lần khi `active` chuyển sang true. */
export const Confetti = ({ active, duration = 3200 }: { active: boolean; duration?: number }) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    if (!active) return;
    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (!canvas || !ctx) return;

    const dpr = Math.min(window.devicePixelRatio || 1, 2);
    const resize = () => {
      canvas.width = window.innerWidth * dpr;
      canvas.height = window.innerHeight * dpr;
      canvas.style.width = `${window.innerWidth}px`;
      canvas.style.height = `${window.innerHeight}px`;
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    };
    resize();
    window.addEventListener("resize", resize);

    const count = window.innerWidth < 640 ? 90 : 160;
    const particles: Particle[] = Array.from({ length: count }, () => ({
      x: Math.random() * window.innerWidth,
      y: -20 - Math.random() * window.innerHeight * 0.4,
      vx: (Math.random() - 0.5) * 4,
      vy: 2 + Math.random() * 3,
      rotation: Math.random() * 360,
      vr: (Math.random() - 0.5) * 10,
      size: 6 + Math.random() * 6,
      color: COLORS[Math.floor(Math.random() * COLORS.length)],
      shape: Math.random() > 0.5 ? "rect" : "circle",
    }));

    let raf = 0;
    let elapsed = 0;
    let last = performance.now();

    const tick = (now: number) => {
      const dt = Math.min(now - last, 32);
      last = now;
      elapsed += dt;

      ctx.clearRect(0, 0, window.innerWidth, window.innerHeight);
      for (const p of particles) {
        p.x += p.vx;
        p.y += p.vy;
        p.vy += 0.03;
        p.rotation += p.vr;

        ctx.save();
        ctx.translate(p.x, p.y);
        ctx.rotate((p.rotation * Math.PI) / 180);
        ctx.fillStyle = p.color;
        if (p.shape === "rect") {
          ctx.fillRect(-p.size / 2, -p.size / 4, p.size, p.size / 2);
        } else {
          ctx.beginPath();
          ctx.arc(0, 0, p.size / 2.5, 0, Math.PI * 2);
          ctx.fill();
        }
        ctx.restore();
      }

      if (elapsed < duration) {
        raf = requestAnimationFrame(tick);
      } else {
        ctx.clearRect(0, 0, window.innerWidth, window.innerHeight);
      }
    };
    raf = requestAnimationFrame(tick);

    return () => {
      cancelAnimationFrame(raf);
      window.removeEventListener("resize", resize);
      ctx.clearRect(0, 0, window.innerWidth, window.innerHeight);
    };
  }, [active, duration]);

  return (
    <canvas
      ref={canvasRef}
      className="pointer-events-none fixed inset-0 z-[100]"
      aria-hidden="true"
    />
  );
};
