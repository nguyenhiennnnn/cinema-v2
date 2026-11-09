import axios from "axios";
import {
  extractApiError,
  getDisplayMessage,
  getFieldErrors,
} from "@/lib/error";

export function normalizeName(value: string): string {
  return value.normalize("NFC").replace(/\s+/g, " ").trim();
}

export function isSameName(a: string, b: string): boolean {
  return (
    normalizeName(a).toLocaleLowerCase("vi") ===
    normalizeName(b).toLocaleLowerCase("vi")
  );
}

export function toSearchKey(value: string): string {
  return normalizeName(value)
    .toLocaleLowerCase("vi")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/đ/g, "d");
}

export function isSameValue(
  a: string | number | null | undefined,
  b: string | number | null | undefined,
): boolean {
  return a != null && b != null && String(a) === String(b);
}

export function hasDisallowedCharacters(value: string): boolean {
  for (const ch of value) {
    const code = ch.codePointAt(0) ?? 0;
    const isControl = code <= 0x1f || (code >= 0x7f && code <= 0x9f);
    const isWhitespace = code === 0x85 || /\s/.test(ch);
    if (isControl && !isWhitespace) return true;
  }
  return false;
}

export function validateNewName(
  name: string,
  maxLength?: number,
): string | null {
  if (hasDisallowedCharacters(name)) {
    return "Tên chứa ký tự không hợp lệ. Xóa ký tự lạ rồi thử lại.";
  }
  if (maxLength && name.length > maxLength) {
    return `Tên dài ${name.length} ký tự, tối đa ${maxLength}. Rút ngắn tên rồi thử lại.`;
  }
  return null;
}

export function filterBySearch<T>(
  options: T[],
  search: string,
  getLabel: (option: T) => string,
): T[] {
  const key = toSearchKey(search);
  if (!key) return options;

  const matched = options.filter((o) => toSearchKey(getLabel(o)).includes(key));
  const exactIndex = matched.findIndex((o) => isSameName(getLabel(o), search));
  if (exactIndex <= 0) return matched;
  return [matched[exactIndex], ...matched.filter((_, i) => i !== exactIndex)];
}

export function describeCreateError(error: unknown): string {
  if (axios.isAxiosError(error) && !error.response) {
    return "Không kết nối được máy chủ. Kiểm tra mạng rồi thử lại.";
  }
  const { statusCode } = extractApiError(error);
  const firstFieldError = Object.values(getFieldErrors(error))[0];
  if (statusCode === 400 && firstFieldError) return firstFieldError;
  return getDisplayMessage(error);
}
