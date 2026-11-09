"use client";

import { useCallback, useRef, useState, useEffect } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { useFieldDisabled } from "@/components/ui/field";
import { Loader2, X, ImageIcon, Film, ImageOff } from "lucide-react";
import type { UseMutationResult } from "@tanstack/react-query";

type MediaKind = "image" | "video";

interface MediaUploaderProps {
  kind: MediaKind;
  folder: string;
  accept: string;
  maxSizeBytes: number;
  value?: string | null;
  onChange: (result: { url: string; publicId: string } | null) => void;
  uploadMutation: UseMutationResult<
    { url: string; publicId: string },
    unknown,
    { file: File; folder: string }
  >;
  className?: string;
}

export const MediaUploader = ({
  kind,
  folder,
  accept,
  maxSizeBytes,
  value,
  onChange,
  uploadMutation,
  className,
}: MediaUploaderProps) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const isDisabled = useFieldDisabled();
  const [isDragging, setIsDragging] = useState(false);
  const [localPreview, setLocalPreview] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [imageError, setImageError] = useState(false);

  const previewUrl = localPreview ?? value ?? null;

  useEffect(() => {
    setImageError(false);
  }, [previewUrl]);

  useEffect(() => {
    return () => {
      if (localPreview) URL.revokeObjectURL(localPreview);
    };
  }, [localPreview]);

  const validateAndUpload = useCallback(
    (file: File) => {
      setError(null);
      setImageError(false);

      if (file.size > maxSizeBytes) {
        setError(
          `File vượt quá giới hạn ${Math.round(maxSizeBytes / (1024 * 1024))}MB.`,
        );
        return;
      }

      const objectUrl = URL.createObjectURL(file);
      setLocalPreview(objectUrl);

      uploadMutation.mutate(
        { file, folder },
        {
          onSuccess: (data) => {
            onChange({ url: data.url, publicId: data.publicId });
          },
          onError: () => {
            setError("Upload thất bại. Vui lòng thử lại.");
            setLocalPreview(null);
          },
        },
      );
    },
    [folder, maxSizeBytes, onChange, uploadMutation],
  );

  const handleFiles = (files: FileList | null) => {
    if (isDisabled) return;
    const file = files?.[0];
    if (file) validateAndUpload(file);
  };

  const handleRemove = () => {
    if (isDisabled) return;
    setLocalPreview(null);
    setError(null);
    setImageError(false);
    onChange(null);
    if (inputRef.current) inputRef.current.value = "";
  };

  return (
    <div className={cn("w-full", className)}>
      <input
        ref={inputRef}
        type="file"
        accept={accept}
        className="hidden"
        onChange={(e) => handleFiles(e.target.files)}
      />

      {previewUrl ? (
        <div className="relative overflow-hidden border border-border bg-muted">
          {kind === "image" ? (
            imageError ? (
              <div className="flex h-40 flex-col items-center justify-center gap-2 p-4 text-center text-muted-foreground">
                <ImageOff className="size-8" />
                <span className="text-xs">Không thể tải ảnh từ URL</span>
                <span className="text-xs text-muted-foreground max-w-xs truncate">{previewUrl}</span>
              </div>
            ) : (
              <img
                src={previewUrl}
                alt="Preview"
                className="max-h-64 w-full object-contain"
                onError={() => setImageError(true)}
              />
            )
          ) : (
            <video src={previewUrl} controls className="max-h-64 w-full" />
          )}

          {uploadMutation.isPending && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/70">
              <Loader2 className="size-6 animate-spin text-primary" />
            </div>
          )}

          <Button
            type="button"
            variant="destructive"
            size="icon"
            className="absolute right-2 top-2 h-7 w-7"
            onClick={handleRemove}
            disabled={uploadMutation.isPending || isDisabled}
          >
            <X className="size-4" />
          </Button>
        </div>
      ) : (
        <div
          role="button"
          tabIndex={isDisabled ? -1 : 0}
          aria-disabled={isDisabled}
          onClick={() => !isDisabled && inputRef.current?.click()}
          onKeyDown={(e) =>
            e.key === "Enter" && !isDisabled && inputRef.current?.click()
          }
          onDragOver={(e) => {
            e.preventDefault();
            setIsDragging(true);
          }}
          onDragLeave={() => setIsDragging(false)}
          onDrop={(e) => {
            e.preventDefault();
            setIsDragging(false);
            handleFiles(e.dataTransfer.files);
          }}
          className={cn(
            "flex flex-col items-center justify-center gap-2 border-2 border-dashed border-border p-8 text-center transition-colors cursor-pointer",
            isDragging && "border-primary bg-accent/10",
            isDisabled && "pointer-events-none cursor-not-allowed opacity-50",
          )}
        >
          {uploadMutation.isPending ? (
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          ) : kind === "image" ? (
            <ImageIcon className="h-8 w-8 text-muted-foreground" />
          ) : (
            <Film className="h-8 w-8 text-muted-foreground" />
          )}
          <p className="text-sm text-muted-foreground">
            Kéo thả {kind === "image" ? "ảnh" : "video"} vào đây hoặc{" "}
            <span className="font-medium text-primary">chọn file</span>
          </p>
          <p className="text-xs text-muted-foreground">
            Tối đa {Math.round(maxSizeBytes / (1024 * 1024))}MB
          </p>
        </div>
      )}

      {error && <p className="mt-2 text-sm text-destructive">{error}</p>}
    </div>
  );
};
