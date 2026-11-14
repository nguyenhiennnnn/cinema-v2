import { queryClient } from "@/api/query-client";
import { invalidateRelated, type InvalidationName } from "@/api/query-invalidation";
import { queryKeys } from "@/api/query-keys";
import { useImportExportJobStore } from "@/stores/import-export-job-store";
import {
  ImportExportJobStatus,
  ImportExportJobType,
  ImportExportModule,
} from "@/types/enum";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { ImportExportJobDto } from "@/types/import-export";
import { importExportApi } from "../api/import-export-api";

type ListResponse = ApiResponse<OffsetPageResult<ImportExportJobDto>>;
type DetailResponse = ApiResponse<ImportExportJobDto>;

const IMPORT_INVALIDATIONS: Record<string, InvalidationName> = {
  [ImportExportModule.Movies]: "movieChanged",
  [ImportExportModule.Showtimes]: "showtimeChanged",
};

const STATUS_RANK: Record<ImportExportJobStatus, number> = {
  [ImportExportJobStatus.Queued]: 0,
  [ImportExportJobStatus.Running]: 1,
  [ImportExportJobStatus.Completed]: 2,
  [ImportExportJobStatus.Failed]: 2,
};

const seenJobIds = new Set<string>();
const finalizedJobIds = new Set<string>();

export const isJobFinished = (job: Pick<ImportExportJobDto, "status">) =>
  job.status === ImportExportJobStatus.Completed ||
  job.status === ImportExportJobStatus.Failed;

export const isJobActive = (job: Pick<ImportExportJobDto, "status">) =>
  !isJobFinished(job);

const isNewerState = (
  previous: ImportExportJobDto | null | undefined,
  next: ImportExportJobDto,
) => {
  if (!previous) return true;
  const previousRank = STATUS_RANK[previous.status];
  const nextRank = STATUS_RANK[next.status];
  if (previousRank !== nextRank) return nextRank > previousRank;
  if (nextRank === STATUS_RANK[ImportExportJobStatus.Completed]) return true;
  return next.progress >= previous.progress;
};

const patchLists = (job: ImportExportJobDto) =>
  queryClient.setQueriesData<ListResponse>(
    { queryKey: queryKeys.adminImportExport.lists() },
    (old) => {
      if (!old) return old;
      const index = old.data.items.findIndex((item) => item.jobId === job.jobId);
      if (index === -1 || !isNewerState(old.data.items[index], job)) return old;
      const items = old.data.items.slice();
      items[index] = job;
      return { ...old, data: { ...old.data, items } };
    },
  );

const finalizeJob = (job: ImportExportJobDto) => {
  if (finalizedJobIds.has(job.jobId)) return;
  finalizedJobIds.add(job.jobId);
  if (job.type !== ImportExportJobType.Import) return;
  const invalidation = IMPORT_INVALIDATIONS[job.module];
  if (invalidation) void invalidateRelated(queryClient, invalidation);
};

export function syncJobToCache(job: ImportExportJobDto): void {
  const detailKey = queryKeys.adminImportExport.detail(job.jobId);
  const cached = queryClient.getQueryData<DetailResponse>(detailKey);
  if (cached && !isNewerState(cached.data, job)) return;

  queryClient.setQueryData<DetailResponse>(detailKey, {
    success: true,
    data: job,
    message: null,
  });
  patchLists(job);

  const isFirstSight = !seenJobIds.has(job.jobId);
  seenJobIds.add(job.jobId);
  const finished = isJobFinished(job);

  if (isFirstSight || finished) {
    void queryClient.invalidateQueries({
      queryKey: queryKeys.adminImportExport.lists(),
    });
  }

  useImportExportJobStore.getState().upsert(job);
  if (finished) finalizeJob(job);
}

const fetchAndSync = (jobId: string) =>
  importExportApi
    .getJob(jobId)
    .then((response) => syncJobToCache(response.data))
    .catch(() => undefined);

export async function trackImportExportJob(jobId: string): Promise<void> {
  useImportExportJobStore.getState().track(jobId);
  await fetchAndSync(jobId);
}

export async function resyncImportExportJobs(): Promise<void> {
  const pending = useImportExportJobStore
    .getState()
    .items.filter((item) => !item.job || isJobActive(item.job));
  await Promise.all(pending.map((item) => fetchAndSync(item.jobId)));
  await queryClient.invalidateQueries({
    queryKey: queryKeys.adminImportExport.lists(),
  });
}

export const hasPendingImportExportJobs = () =>
  useImportExportJobStore
    .getState()
    .items.some((item) => !item.job || isJobActive(item.job));
