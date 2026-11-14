import { ENDPOINTS } from "@/api/endpoints";
import type { ImportWizardConfig } from "@/components/common/import-wizard/import-wizard-types";

import type { UpdateSeatLayoutResponse } from "../types";

export const createSeatLayoutImportConfig = (roomId: number): ImportWizardConfig => ({
  title: "Import sơ đồ ghế từ file",
  description:
    "Nhập sơ đồ ghế từ file Excel hoặc CSV. File thay thế toàn bộ sơ đồ hiện tại của phòng, ghế không có trong file sẽ bị gỡ hoặc vô hiệu hoá.",
  baseUrl: ENDPOINTS.admin.rooms.seatLayoutImport(roomId),
  itemLabel: "ghế",
  templateFilename: "seat-layout-import-template.xlsx",
  errorsFilename: "seat-layout-import-errors.xlsx",
  invalidates: "seatLayoutChanged",
  columns: [
    { key: "SeatRow", header: "Hàng" },
    { key: "SeatNumber", header: "Số ghế" },
    { key: "SeatType", header: "Loại ghế" },
    { key: "IsActive", header: "Đang bán" },
    { key: "IsAisle", header: "Lối đi" },
    { key: "CoupleGroup", header: "Nhóm ghế đôi" },
  ],
  summarizeApply: (data) => {
    const response = data as UpdateSeatLayoutResponse;
    return {
      message: "Đã cập nhật sơ đồ ghế của phòng.",
      metrics: [
        { label: "Tạo mới", value: response.created },
        { label: "Cập nhật", value: response.updated },
        { label: "Vô hiệu hoá", value: response.deactivated },
        { label: "Đã xoá", value: response.removed },
        { label: "Tổng ghế bán được", value: response.totalSeats },
      ],
    };
  },
});
