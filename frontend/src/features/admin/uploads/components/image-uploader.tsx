"use client";

import { MediaUploader } from "./media-uploader";
import { useUploadImage } from "../hooks/use-uploads";
import {
  ACCEPTED_IMAGE_ACCEPT,
  MAX_IMAGE_SIZE_BYTES,
} from "@/config/default-constants";

interface ImageUploaderProps {
  folder: string;
  value?: string | null;
  onChange: (result: { url: string; publicId: string } | null) => void;
  maxSizeBytes?: number;
  className?: string;
}

export const ImageUploader = ({
  folder,
  value,
  onChange,
  maxSizeBytes = MAX_IMAGE_SIZE_BYTES,
  className,
}: ImageUploaderProps) => {
  const uploadMutation = useUploadImage();

  return (
    <MediaUploader
      kind="image"
      folder={folder}
      accept={ACCEPTED_IMAGE_ACCEPT}
      maxSizeBytes={maxSizeBytes}
      value={value}
      onChange={onChange}
      uploadMutation={uploadMutation}
      className={className}
    />
  );
};
