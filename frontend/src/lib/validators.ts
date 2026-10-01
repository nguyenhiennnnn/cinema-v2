import { z } from "zod";

// ─── Email ────────────────────────────────────────────────────────────────────

export const emailSchema = z
  .string()
  .min(1, "Vui lòng nhập email.")
  .email("Email không hợp lệ.");

// ─── Password (strong) ────────────────────────────────────────────────────────
// Rules: min 8 chars, uppercase + lowercase, at least 1 digit or special char
// Score >= 3 of: length, case-mix, digit, special char

export const passwordSchema = z
  .string()
  .min(8, "Mật khẩu phải có ít nhất 8 ký tự.")
  .refine((v) => /[a-z]/.test(v) && /[A-Z]/.test(v), {
    message: "Mật khẩu phải có cả chữ hoa và chữ thường.",
  })
  .refine(
    (v) => {
      let score = 0;
      if (v.length >= 8) score++;
      if (/[a-z]/.test(v) && /[A-Z]/.test(v)) score++;
      if (/\d/.test(v)) score++;
      if (/[^a-zA-Z0-9]/.test(v)) score++;
      return score >= 3;
    },
    {
      message:
        "Mật khẩu chưa đủ mạnh. Vui lòng thêm số hoặc ký tự đặc biệt.",
    },
  );
