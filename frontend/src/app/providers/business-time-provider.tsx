import { Fragment, useEffect, useState, useSyncExternalStore } from "react";
import { PageLoader } from "@/components/common/spinner";
import { BUSINESS_TIME_ZONE_BOOT_TIMEOUT_MS } from "@/config/default-constants";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";
import {
  getBusinessTimeZone,
  subscribeBusinessTimeZone,
} from "@/lib/business-time";

export const BusinessTimeProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const { isPending } = useCinemaSettings();
  const timeZone = useSyncExternalStore(
    subscribeBusinessTimeZone,
    getBusinessTimeZone,
  );
  const [timedOut, setTimedOut] = useState(false);

  useEffect(() => {
    if (!isPending) return;
    const id = window.setTimeout(
      () => setTimedOut(true),
      BUSINESS_TIME_ZONE_BOOT_TIMEOUT_MS,
    );
    return () => window.clearTimeout(id);
  }, [isPending]);

  if (isPending && !timedOut) return <PageLoader />;

  return <Fragment key={timeZone}>{children}</Fragment>;
};
