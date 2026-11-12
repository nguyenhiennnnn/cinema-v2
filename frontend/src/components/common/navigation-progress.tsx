import { useEffect, useState } from "react";
import { useNavigation } from "react-router";

const SHOW_DELAY_MS = 120;

export const NavigationProgress = () => {
  const { state } = useNavigation();
  const isNavigating = state !== "idle";
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    if (!isNavigating) {
      setVisible(false);
      return;
    }
    const id = window.setTimeout(() => setVisible(true), SHOW_DELAY_MS);
    return () => window.clearTimeout(id);
  }, [isNavigating]);

  if (!visible) return null;

  return (
    <div
      role="progressbar"
      aria-busy="true"
      aria-label="Đang chuyển trang"
      className="pointer-events-none fixed inset-x-0 top-0 z-100 h-0.5 overflow-hidden bg-primary/20"
    >
      <div className="h-full w-1/3 animate-[navigation-progress_1s_ease-in-out_infinite] bg-primary" />
    </div>
  );
};
