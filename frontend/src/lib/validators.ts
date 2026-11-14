import { z } from "zod";
import { VN_PHONE_REGEX } from "@/config/default-constants";

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


export const strongPasswordSchema = passwordSchema.refine((v) => /\d/.test(v), {
  message: "Mật khẩu phải có ít nhất 1 chữ số.",
});

export const isValidVnPhone = (value: string) => VN_PHONE_REGEX.test(value);
