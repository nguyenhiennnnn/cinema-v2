import { useCallback, useRef } from "react";

export function useCheckinSound() {
  const contextRef = useRef<AudioContext | null>(null);

  const getContext = useCallback(() => {
    if (!contextRef.current) {
      contextRef.current = new AudioContext();
    }
    return contextRef.current;
  }, []);

  const playTone = useCallback(
    (frequency: number, duration: number) => {
      const ctx = getContext();
      const oscillator = ctx.createOscillator();
      const gain = ctx.createGain();
      oscillator.type = "sine";
      oscillator.frequency.value = frequency;
      gain.gain.setValueAtTime(0.2, ctx.currentTime);
      gain.gain.exponentialRampToValueAtTime(
        0.0001,
        ctx.currentTime + duration,
      );
      oscillator.connect(gain);
      gain.connect(ctx.destination);
      oscillator.start();
      oscillator.stop(ctx.currentTime + duration);
    },
    [getContext],
  );

  const playSuccess = useCallback(() => playTone(800, 0.15), [playTone]);
  const playError = useCallback(() => playTone(200, 0.35), [playTone]);

  return { playSuccess, playError };
}
