import { useEffect, useRef } from "react";
import { useBlocker } from "react-router";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

export function useUnsavedChangesGuard(isDirty: boolean) {
  const openAlert = useAlertDialogOpen();
  const blocker = useBlocker(
    ({ currentLocation, nextLocation }) =>
      isDirty && currentLocation.pathname !== nextLocation.pathname,
  );
  const blockerRef = useRef(blocker);

  useEffect(() => {
    blockerRef.current = blocker;
  });

  useEffect(() => {
    if (blocker.state !== "blocked") return;
    openAlert({
      title: "Rời trang khi chưa lưu?",
      description: "Sơ đồ ghế có thay đổi chưa lưu. Nếu rời đi, các thay đổi này sẽ bị mất.",
      variant: "destructive",
      confirmText: "Rời đi",
      cancelText: "Ở lại chỉnh tiếp",
      onConfirm: () => {
        const current = blockerRef.current;
        if (current.state === "blocked") current.proceed();
      },
      onCancel: () => {
        const current = blockerRef.current;
        if (current.state === "blocked") current.reset();
      },
    });
  }, [blocker.state, openAlert]);

  useEffect(() => {
    if (!isDirty) return;
    const handler = (e: BeforeUnloadEvent) => {
      e.preventDefault();
      e.returnValue = "";
    };
    window.addEventListener("beforeunload", handler);
    return () => window.removeEventListener("beforeunload", handler);
  }, [isDirty]);
}
