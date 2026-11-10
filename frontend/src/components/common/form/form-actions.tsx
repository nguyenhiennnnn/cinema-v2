import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { FORM_CANCEL_LABEL, FORM_PENDING_LABEL } from "@/config/default-constants";
import { useBlockDialogClose } from "@/hooks/use-block-dialog-close";
import {
  useDiscardConfirm,
  useUnsavedChangesGuard,
} from "@/components/common/modal-guard";
import { SubmitButton } from "./submit-button";

interface FormActionsProps {
  isPending: boolean;
  isDirty?: boolean;
  submitLabel: string;
  pendingLabel?: string;
  cancelLabel?: string;
  onCancel: () => void;
}

export const FormActions = ({
  isPending,
  isDirty = false,
  submitLabel,
  pendingLabel = FORM_PENDING_LABEL,
  cancelLabel = FORM_CANCEL_LABEL,
  onCancel,
}: FormActionsProps) => {
  useBlockDialogClose(isPending);
  useUnsavedChangesGuard(isDirty);
  const confirmDiscard = useDiscardConfirm(isDirty);

  return (
    <div
      data-slot="form-actions"
      className={cn(
        "sticky bottom-0 z-10 flex justify-end gap-2 border-t border-border py-3",
        "-mx-(--modal-gutter,0px) -mb-(--modal-gutter,0px) px-(--modal-gutter,0px)",
        "bg-(--modal-surface,var(--background))",
      )}
    >
      <Button
        type="button"
        variant="outline"
        onClick={() => confirmDiscard(onCancel)}
        disabled={isPending}
      >
        {cancelLabel}
      </Button>
      <SubmitButton
        isPending={isPending}
        label={submitLabel}
        pendingLabel={pendingLabel}
      />
    </div>
  );
};
