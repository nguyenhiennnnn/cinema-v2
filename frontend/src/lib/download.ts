import axios from "axios";

const UTF8_FILENAME_PATTERN = /filename\*\s*=\s*UTF-8''([^;]+)/i;
const PLAIN_FILENAME_PATTERN = /filename\s*=\s*(?:"([^"]+)"|([^;]+))/i;

export function getFilenameFromContentDisposition(
  header: unknown,
): string | null {
  if (typeof header !== "string" || header === "") return null;

  const encoded = UTF8_FILENAME_PATTERN.exec(header)?.[1];
  if (encoded) {
    try {
      return decodeURIComponent(encoded.trim());
    } catch {
      return encoded.trim();
    }
  }

  const match = PLAIN_FILENAME_PATTERN.exec(header);
  const plain = match?.[1] ?? match?.[2];
  return plain ? plain.trim() : null;
}

export function saveBlob(blob: Blob, filename: string): void {
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  link.style.display = "none";
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.setTimeout(() => URL.revokeObjectURL(url), 0);
}

export async function normalizeBlobError(error: unknown): Promise<unknown> {
  if (!axios.isAxiosError(error)) return error;

  const data: unknown = error.response?.data;
  if (!(data instanceof Blob) || !error.response) return error;

  try {
    error.response.data = JSON.parse(await data.text());
  } catch {
    error.response.data = undefined;
  }
  return error;
}
