import { useEffect, useState } from "react";
import { NOW_TICK_INTERVAL_MS } from "@/config/default-constants";

export function useNow(intervalMs = NOW_TICK_INTERVAL_MS) {
  const [now, setNow] = useState(() => Date.now());

  useEffect(() => {
    const id = setInterval(() => setNow(Date.now()), intervalMs);
    return () => clearInterval(id);
  }, [intervalMs]);

  return now;
}
