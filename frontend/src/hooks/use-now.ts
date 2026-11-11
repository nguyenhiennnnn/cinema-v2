import { useEffect, useState } from "react";
import { NOW_TICK_INTERVAL_MS } from "@/config/default-constants";
import { getServerNow } from "@/lib/server-clock";

export function useNow(intervalMs = NOW_TICK_INTERVAL_MS) {
  const [now, setNow] = useState(() => getServerNow());

  useEffect(() => {
    const id = setInterval(() => setNow(getServerNow()), intervalMs);
    return () => clearInterval(id);
  }, [intervalMs]);

  return now;
}
