import { Link } from "react-router";

import { BrandMark } from "@/components/common/brand-mark";

export const HeaderLogo = () => (
  <Link to="/" className="group flex items-center gap-2">
    <BrandMark className="size-8 transition-transform group-hover:scale-105" />
    <span className="font-heading text-xl leading-none font-semibold tracking-tight text-foreground uppercase">
      Cinema
    </span>
  </Link>
);