import { create } from "zustand";
import { IMPORT_EXPORT_PANEL_MAX_ITEMS } from "@/config/default-constants";
import type { ImportExportJobDto } from "@/types/import-export";

export interface TrackedImportExportJob {
  jobId: string;
  job: ImportExportJobDto | null;
}

interface ImportExportJobStore {
  items: TrackedImportExportJob[];
  expanded: boolean;
  track: (jobId: string) => void;
  upsert: (job: ImportExportJobDto) => void;
  dismiss: (jobId: string) => void;
  dismissFinished: () => void;
  setExpanded: (expanded: boolean) => void;
  reset: () => void;
}

const isFinished = (job: ImportExportJobDto | null) =>
  job?.status === "Completed" || job?.status === "Failed";

export const useImportExportJobStore = create<ImportExportJobStore>()(
  (set) => ({
    items: [],
    expanded: true,

    track: (jobId) =>
      set((state) =>
        state.items.some((item) => item.jobId === jobId)
          ? { expanded: true }
          : {
              expanded: true,
              items: [{ jobId, job: null }, ...state.items].slice(
                0,
                IMPORT_EXPORT_PANEL_MAX_ITEMS,
              ),
            },
      ),

    upsert: (job) =>
      set((state) =>
        state.items.some((item) => item.jobId === job.jobId)
          ? {
              items: state.items.map((item) =>
                item.jobId === job.jobId ? { jobId: job.jobId, job } : item,
              ),
            }
          : {
              items: [{ jobId: job.jobId, job }, ...state.items].slice(
                0,
                IMPORT_EXPORT_PANEL_MAX_ITEMS,
              ),
            },
      ),

    dismiss: (jobId) =>
      set((state) => ({
        items: state.items.filter((item) => item.jobId !== jobId),
      })),

    dismissFinished: () =>
      set((state) => ({
        items: state.items.filter((item) => !isFinished(item.job)),
      })),

    setExpanded: (expanded) => set({ expanded }),

    reset: () => set({ items: [], expanded: true }),
  }),
);
