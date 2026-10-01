"use client";

import { MediaUploader } from "./media-uploader";
import { useUploadVideo } from "../hooks/use-uploads";
import {
  ACCEPTED_VIDEO_ACCEPT,
  MAX_VIDEO_SIZE_BYTES,
} from "@/config/default-constants";

interface VideoUploaderProps {
  folder: string;
  value?: string | null;
  onChange: (result: { url: string; publicId: string } | null) => void;
  maxSizeBytes?: number;
  className?: string;
}

export const VideoUploader = ({
  folder,
  value,
  onChange,
  maxSizeBytes = MAX_VIDEO_SIZE_BYTES,
  className,
}: VideoUploaderProps) => {
  const uploadMutation = useUploadVideo();

  return (
    <MediaUploader
      kind="video"
      folder={folder}
      accept={ACCEPTED_VIDEO_ACCEPT}
      maxSizeBytes={maxSizeBytes}
      value={value}
      onChange={onChange}
      uploadMutation={uploadMutation}
      className={className}
    />
  );
};
