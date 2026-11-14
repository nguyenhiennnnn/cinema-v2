import { HubConnectionState } from "@microsoft/signalr";
import { useEffect } from "react";
import { createHubConnection } from "@/lib/signalr-connection";
import { SignalREvents } from "@/lib/signalr";
import { ENV } from "@/config/env";
import { IMPORT_EXPORT_RESYNC_INTERVAL_MS } from "@/config/default-constants";
import {
  hasPendingImportExportJobs,
  resyncImportExportJobs,
  syncJobToCache,
} from "@/features/admin/import-export/lib/job-sync";
import { useImportExportJobStore } from "@/stores/import-export-job-store";
import type { ImportExportJobDto } from "@/types/import-export";

export function useImportExportJobHub() {
  useEffect(() => {
    const connection = createHubConnection();
    const handleJob = (job: ImportExportJobDto) => syncJobToCache(job);

    connection.on(SignalREvents.ImportExportJobProgress, handleJob);
    connection.on(SignalREvents.ImportExportJobCompleted, handleJob);
    connection.on(SignalREvents.ImportExportJobFailed, handleJob);
    connection.onreconnected(() => {
      void resyncImportExportJobs();
    });

    connection.start().catch((err) => {
      if (ENV.IS_DEV) {
        console.warn("[SignalR] Kết nối theo dõi tác vụ nền thất bại:", err);
      }
    });

    const resyncTimer = window.setInterval(() => {
      if (connection.state === HubConnectionState.Connected) return;
      if (!hasPendingImportExportJobs()) return;
      void resyncImportExportJobs();
    }, IMPORT_EXPORT_RESYNC_INTERVAL_MS);

    return () => {
      window.clearInterval(resyncTimer);
      void connection.stop();
      useImportExportJobStore.getState().reset();
    };
  }, []);
}
