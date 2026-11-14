import { useDialogOpen } from "@/stores/dialog-store";

import { ImportExportJobDetail } from "../components/import-export-job-detail";

export function useImportExportJobDetailDialog() {
  const openDialog = useDialogOpen();

  return (jobId: string) =>
    openDialog({
      title: "Chi tiết tác vụ",
      description: "Tiến độ, kết quả và lỗi của tác vụ nhập/xuất dữ liệu.",
      content: <ImportExportJobDetail jobId={jobId} />,
    });
}
