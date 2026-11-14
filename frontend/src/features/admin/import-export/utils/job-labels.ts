import { ImportExportJobType } from "@/types/enum";
import type { ImportExportJobDto } from "@/types/import-export";

const MODULE_LABELS: Record<string, string> = {
  movies: "Phim",
  showtimes: "Suất chiếu",
  bookings: "Đặt vé",
  payments: "Thanh toán",
};

export const getJobModuleLabel = (module: string) =>
  MODULE_LABELS[module] ?? module;

export const getJobTypeLabel = (type: string) =>
  type === ImportExportJobType.Import ? "Nhập" : "Xuất";

export const getJobTitle = (job: Pick<ImportExportJobDto, "type" | "module">) =>
  `${getJobTypeLabel(job.type)} ${getJobModuleLabel(job.module).toLowerCase()}`;

export const getJobFormatLabel = (format: string) => format.toUpperCase();

export const getJobCreatorLabel = (
  createdByUserId: number,
  currentUserId?: number | null,
) =>
  createdByUserId === currentUserId
    ? "Bạn"
    : `Người dùng #${createdByUserId}`;
