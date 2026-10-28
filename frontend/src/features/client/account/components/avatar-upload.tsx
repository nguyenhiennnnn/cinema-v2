import { useRef, useState } from "react";
import { Camera, Loader2 } from "lucide-react";
import { UserAvatar } from "@/components/common/user-avatar";
import {
  ACCEPTED_IMAGE_ACCEPT,
  MAX_IMAGE_SIZE_BYTES,
} from "@/config/default-constants";
import { useUploadImage } from "@/features/admin/uploads/hooks/use-uploads";

interface AvatarUploadProps {
  value: string | null;
  name: string;
  onChange: (url: string | null) => void;
  folder?: string;
}

export const AvatarUpload = ({
  value,
  name,
  onChange,
  folder = "avatars",
}: AvatarUploadProps) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const [error, setError] = useState<string | null>(null);
  const uploadMutation = useUploadImage();

  const handleFile = (file: File) => {
    setError(null);

    if (file.size > MAX_IMAGE_SIZE_BYTES) {
      setError(
        `File vượt quá giới hạn ${Math.round(MAX_IMAGE_SIZE_BYTES / (1024 * 1024))}MB.`,
      );
      return;
    }

    uploadMutation.mutate(
      { file, folder },
      {
        onSuccess: (data) => onChange(data.url),
        onError: () => setError("Upload thất bại. Vui lòng thử lại."),
      },
    );
  };

  return (
    <div className="flex flex-col items-center gap-1.5">
      <div className="relative">
        <UserAvatar src={value} name={name} size="lg" className="size-20" />

        {uploadMutation.isPending && (
          <div className="absolute inset-0 flex items-center justify-center rounded-full bg-background/70">
            <Loader2 className="size-5 animate-spin text-primary" />
          </div>
        )}

        <button
          type="button"
          onClick={() => inputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="absolute -right-1 -bottom-1 flex size-7 items-center justify-center cut-xs bg-secondary text-foreground hover:bg-muted disabled:opacity-50"
        >
          <Camera className="size-3.5" />
        </button>

        <input
          ref={inputRef}
          type="file"
          accept={ACCEPTED_IMAGE_ACCEPT}
          className="hidden"
          onChange={(e) => {
            const file = e.target.files?.[0];
            if (file) handleFile(file);
            e.target.value = "";
          }}
        />
      </div>

      {error && <p className="text-xs text-destructive">{error}</p>}
    </div>
  );
};