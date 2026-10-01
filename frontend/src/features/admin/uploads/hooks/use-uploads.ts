import { useMutation } from "@tanstack/react-query";
import { uploadsApi } from "../api/uploads-api";

export function useUploadImage() {
  return useMutation({
    mutationFn: async ({ file, folder }: { file: File; folder: string }) => {
      const res = await uploadsApi.uploadImage(file, folder);
      return res.data;
    },
  });
}

export function useUploadVideo() {
  return useMutation({
    mutationFn: async ({ file, folder }: { file: File; folder: string }) => {
      const res = await uploadsApi.uploadVideo(file, folder);
      return res.data;
    },
  });
}
