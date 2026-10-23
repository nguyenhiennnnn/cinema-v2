import { Link } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";

export const HeaderLogo = () => (
  <Link to="/" className="group flex items-center gap-2">
    <BrandMark className="size-8 shrink-0" />
    <BrandWordmark className="hidden h-5 w-auto sm:block" />
  </Link>
);
