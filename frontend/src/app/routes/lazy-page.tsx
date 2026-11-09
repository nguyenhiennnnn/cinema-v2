import { Suspense } from "react";
import { RouteFallback } from "@/components/common/route-fallback";

export function LazyPage({ children }: { children: React.ReactNode }) {
  return <Suspense fallback={<RouteFallback />}>{children}</Suspense>;
}
