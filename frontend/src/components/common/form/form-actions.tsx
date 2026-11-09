import { Button } from "@/components/ui/button";
import { FORM_CANCEL_LABEL, FORM_PENDING_LABEL } from "@/config/default-constants";
import { SubmitButton } from "./submit-button";

interface FormActionsProps {
  isPending: boolean;
  submitLabel: string;
  pendingLabel?: string;
  cancelLabel?: string;
  onCancel: () => void;
}

export const FormActions = ({
  isPending,
  submitLabel,
  pendingLabel = FORM_PENDING_LABEL,
  cancelLabel = FORM_CANCEL_LABEL,
  onCancel,
}: FormActionsProps) => (
  <div className="flex justify-end gap-2 pt-2">
    <Button
      type="button"
      variant="outline"
      onClick={onCancel}
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
