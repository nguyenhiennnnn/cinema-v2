import { useEffect, useState } from "react";
import { Spinner } from "@/components/common/spinner";

const DEFAULT_DELAY_MS = 300;

interface RouteFallbackProps {
  delay?: number;
}

export const RouteFallback = ({ delay = DEFAULT_DELAY_MS }: RouteFallbackProps) => {
  const [visible, setVisible] = useState(delay <= 0);

  useEffect(() => {
    if (visible) return;
    const id = window.setTimeout(() => setVisible(true), delay);
    return () => window.clearTimeout(id);
  }, [delay, visible]);

  if (!visible) return null;

  return (
    <div className="flex min-h-[40vh] items-center justify-center">
      <Spinner />
    </div>
  );
};
