const HEADER_RESOLUTION_MS = 1000;
const WINDOW_SIZE = 12;
const MAX_TRUSTED_RTT_MS = 3000;
const OUTLIER_THRESHOLD_MS = 5000;
const OUTLIER_RESET_COUNT = 3;
const OUTLIER_CONSISTENCY_MS = 2000;

interface Sample {
  offsetMs: number;
  rttMs: number;
}

let samples: Sample[] = [];
let offsetMs = 0;
let synced = false;
let pendingOutliers: number[] = [];

function median(values: number[]): number {
  const sorted = [...values].sort((a, b) => a - b);
  const mid = Math.floor(sorted.length / 2);
  return sorted.length % 2 === 0
    ? (sorted[mid - 1] + sorted[mid]) / 2
    : sorted[mid];
}

function recomputeOffset() {
  const byRtt = [...samples].sort((a, b) => a.rttMs - b.rttMs);
  const best = byRtt.slice(0, Math.max(1, Math.ceil(byRtt.length / 2)));
  offsetMs = Math.round(median(best.map((s) => s.offsetMs)));
}

function addSample(sample: Sample) {
  samples.push(sample);
  if (samples.length > WINDOW_SIZE) samples.shift();
  recomputeOffset();
  synced = true;
}

export function syncServerClock(
  dateHeader: string | null | undefined,
  rttMs?: number,
): void {
  if (!dateHeader) return;
  const headerMs = Date.parse(dateHeader);
  if (Number.isNaN(headerMs)) return;

  const rtt =
    rttMs !== undefined && Number.isFinite(rttMs) && rttMs >= 0
      ? rttMs
      : MAX_TRUSTED_RTT_MS;

  if (synced && rtt > MAX_TRUSTED_RTT_MS) return;

  const clientAtHeader = Date.now() - rtt / 2;
  const serverAtHeader = headerMs + HEADER_RESOLUTION_MS / 2;
  const candidate = serverAtHeader - clientAtHeader;

  if (synced && Math.abs(candidate - offsetMs) > OUTLIER_THRESHOLD_MS) {
    const consistent =
      pendingOutliers.length === 0 ||
      Math.abs(candidate - pendingOutliers[pendingOutliers.length - 1]) <=
        OUTLIER_CONSISTENCY_MS;
    pendingOutliers = consistent ? [...pendingOutliers, candidate] : [candidate];

    if (pendingOutliers.length < OUTLIER_RESET_COUNT) return;

    samples = [];
    synced = false;
  }

  pendingOutliers = [];
  addSample({ offsetMs: candidate, rttMs: rtt });
}

export function getServerNow(): number {
  return Date.now() + offsetMs;
}

export function getServerClockOffsetMs(): number {
  return offsetMs;
}
