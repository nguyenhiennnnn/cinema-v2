import { ENDPOINTS } from "@/api/endpoints";
import type { ImportWizardConfig } from "@/components/common/import-wizard/import-wizard-types";

export const movieImportConfig: ImportWizardConfig = {
  title: "Import phim từ file",
  description:
    "Nhập hàng loạt phim từ file Excel hoặc CSV. Phim trùng tên và ngày khởi chiếu sẽ được cập nhật.",
  baseUrl: ENDPOINTS.admin.movies.import,
  itemLabel: "phim",
  templateFilename: "movies-import-template.xlsx",
  errorsFilename: "movies-import-errors.xlsx",
  invalidates: "movieChanged",
  columns: [
    { key: "Title", header: "Tên phim" },
    { key: "OriginalTitle", header: "Tên gốc" },
    { key: "Description", header: "Mô tả" },
    { key: "Director", header: "Đạo diễn" },
    { key: "DurationMinutes", header: "Thời lượng" },
    { key: "ReleaseDate", header: "Khởi chiếu" },
    { key: "Language", header: "Ngôn ngữ" },
    { key: "Subtitle", header: "Phụ đề" },
    { key: "AgeRating", header: "Độ tuổi" },
    { key: "PosterUrl", header: "Poster" },
    { key: "TrailerUrl", header: "Trailer" },
    { key: "Status", header: "Trạng thái" },
    { key: "Genres", header: "Thể loại" },
    { key: "Cast", header: "Diễn viên" },
  ],
};
