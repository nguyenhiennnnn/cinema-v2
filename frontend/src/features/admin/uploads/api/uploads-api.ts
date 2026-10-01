import { apiUpload } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { UploadImageResponse } from "../types";

export const uploadsApi = {
  uploadImage: (file: File, folder = "misc") => {
    const formData = new FormData();
    formData.append("file", file);
    return apiUpload<ApiResponse<UploadImageResponse>>(
      `/admin/uploads/image?folder=${encodeURIComponent(folder)}`,
      formData,
    );
  },

  uploadVideo: (file: File, folder = "misc") => {
    const formData = new FormData();
    formData.append("file", file);
    return apiUpload<ApiResponse<UploadImageResponse>>(
      `/admin/uploads/video?folder=${encodeURIComponent(folder)}`,
      formData,
    );
  },
};
