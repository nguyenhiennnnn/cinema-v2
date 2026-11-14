import { z } from "zod";
import { VN_PHONE_REGEX } from "@/config/default-constants";

export const PASSWORD_MIN_LENGTH = 8;
export const PASSWORD_UPPERCASE_REGEX = /[A-Z]/;
export const PASSWORD_LOWERCASE_REGEX = /[a-z]/;
export const PASSWORD_DIGIT_REGEX = /[0-9]/;

export const strongPasswordSchema = z
  .string()
  .min(PASSWORD_MIN_LENGTH, "Mật khẩu phải có ít nhất 8 ký tự.")
  .regex(PASSWORD_UPPERCASE_REGEX, "Mật khẩu phải có ít nhất 1 chữ hoa.")
  .regex(PASSWORD_LOWERCASE_REGEX, "Mật khẩu phải có ít nhất 1 chữ thường.")
  .regex(PASSWORD_DIGIT_REGEX, "Mật khẩu phải có ít nhất 1 chữ số.");

export const DEFAULT_EMPLOYEE_PASSWORD_MAX_LENGTH = 100;

export const defaultEmployeePasswordSchema = strongPasswordSchema.max(
  DEFAULT_EMPLOYEE_PASSWORD_MAX_LENGTH,
  `Mật khẩu mặc định tối đa ${DEFAULT_EMPLOYEE_PASSWORD_MAX_LENGTH} ký tự.`,
);

export const loginPasswordSchema = z.string().min(1, "Vui lòng nhập mật khẩu.");

export const isValidVnPhone = (value: string) => VN_PHONE_REGEX.test(value);
