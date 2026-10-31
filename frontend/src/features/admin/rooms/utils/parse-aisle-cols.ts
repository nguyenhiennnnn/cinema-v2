export const parseAisleCols = (text: string | undefined) =>
  (text ?? "")
    .split(",")
    .map((s) => Number(s.trim()))
    .filter((n) => Number.isInteger(n) && n > 0);
