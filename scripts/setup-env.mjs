import { copyFileSync, existsSync } from "node:fs";

for (const [src, dest] of [
  [".env.example", ".env"],
  ["frontend/.env.example", "frontend/.env"],
]) {
  if (existsSync(src) && !existsSync(dest)) {
    copyFileSync(src, dest);
    console.log(`created ${dest}`);
  }
}
