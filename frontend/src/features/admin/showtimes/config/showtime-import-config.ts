import { ENDPOINTS } from "@/api/endpoints";
import type { ImportWizardConfig } from "@/components/common/import-wizard/import-wizard-types";

export const showtimeImportConfig: ImportWizardConfig = {
  title: "Import suất chiếu từ file",
  description:
    "Nhập hàng loạt suất chiếu từ file Excel hoặc CSV. Dòng vi phạm quy tắc hoặc trùng lịch sẽ bị bỏ qua kèm lý do.",
  baseUrl: ENDPOINTS.admin.showtimes.import,
  itemLabel: "suất chiếu",
  templateFilename: "showtimes-import-template.xlsx",
  errorsFilename: "showtimes-import-errors.xlsx",
  invalidates: "showtimeChanged",
  columns: [
    { key: "Movie", header: "Phim" },
    { key: "Room", header: "Phòng" },
    { key: "StartTime", header: "Giờ chiếu" },
    { key: "BasePrice", header: "Giá vé cơ bản" },
    { key: "AllowAfterHours", header: "Sau giờ đóng cửa" },
  ],
};
