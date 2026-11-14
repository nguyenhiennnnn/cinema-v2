import {
  PASSWORD_DIGIT_REGEX,
  PASSWORD_LOWERCASE_REGEX,
  PASSWORD_MIN_LENGTH,
  PASSWORD_UPPERCASE_REGEX,
} from "@/lib/validators";

const PASSWORD_SPECIAL_REGEX = /[^a-zA-Z0-9]/;

export function scorePassword(password: string) {
  let score = 0;
  if (password.length >= PASSWORD_MIN_LENGTH) score++;
  if (PASSWORD_LOWERCASE_REGEX.test(password) && PASSWORD_UPPERCASE_REGEX.test(password)) score++;
  if (PASSWORD_DIGIT_REGEX.test(password)) score++;
  if (PASSWORD_SPECIAL_REGEX.test(password)) score++;
  return score;
}
