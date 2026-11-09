import { createContext, useContext, useEffect } from "react";

export interface DismissGuard {
  block: () => void;
  release: () => void;
}

export const DismissGuardContext = createContext<DismissGuard | null>(null);

export const useBlockDialogClose = (isBusy: boolean) => {
  const guard = useContext(DismissGuardContext);

  useEffect(() => {
    if (!guard || !isBusy) return;
    guard.block();
    return () => guard.release();
  }, [guard, isBusy]);
};
