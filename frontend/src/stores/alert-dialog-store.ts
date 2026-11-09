import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import { logDevError } from "@/lib/error";

interface AlertDialogOptions {
  title: string;
  description?: string;
  confirmText?: string;
  cancelText?: string;
  variant?: "default" | "destructive";
  onConfirm: () => void | Promise<void>;
  onCancel?: () => void;
}

interface AlertDialogStore extends AlertDialogOptions {
  isOpen: boolean;
  isLoading: boolean;
  open: (options: AlertDialogOptions) => void;
  close: () => void;
  confirm: () => Promise<void>;
}

export const useAlertDialogStore = create<AlertDialogStore>((set, get) => ({
  isOpen: false,
  isLoading: false,
  title: "",
  description: undefined,
  confirmText: "Xác nhận",
  cancelText: "Hủy",
  variant: "default",
  onConfirm: () => {},
  onCancel: undefined,
  open: ({
    title,
    description,
    confirmText = "Xác nhận",
    cancelText = "Hủy",
    variant = "default",
    onConfirm,
    onCancel,
  }) =>
    set({
      isOpen: true,
      title,
      description,
      confirmText,
      cancelText,
      variant,
      onConfirm,
      onCancel,
    }),
  close: () => {
    get().onCancel?.();
    set({ isOpen: false, isLoading: false });
  },
  confirm: async () => {
    try {
      set({ isLoading: true });
      await get().onConfirm();
      set({ isOpen: false, isLoading: false });
    } catch (error) {
      logDevError(error, "AlertDialog");
      set({ isLoading: false });
    }
  },
}));

export const useAlertDialogOpen = () => useAlertDialogStore((s) => s.open);

export const useAlertDialogState = () =>
  useAlertDialogStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      isLoading: s.isLoading,
      title: s.title,
      description: s.description,
      confirmText: s.confirmText,
      cancelText: s.cancelText,
      variant: s.variant,
      close: s.close,
      confirm: s.confirm,
    })),
  );
