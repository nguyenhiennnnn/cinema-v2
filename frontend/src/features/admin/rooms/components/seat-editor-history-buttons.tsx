import { Redo2, Undo2 } from "lucide-react";
import { Button } from "@/components/ui/button";

interface SeatEditorHistoryButtonsProps {
  disabled: boolean;
  canUndo: boolean;
  canRedo: boolean;
  onUndo: () => void;
  onRedo: () => void;
}

export const SeatEditorHistoryButtons = ({
  disabled,
  canUndo,
  canRedo,
  onUndo,
  onRedo,
}: SeatEditorHistoryButtonsProps) => (
  <div className="flex items-center gap-1">
    <Button
      type="button"
      variant="outline"
      size="icon"
      title="Hoàn tác"
      disabled={disabled || !canUndo}
      onClick={onUndo}
    >
      <Undo2 className="size-4" />
    </Button>
    <Button
      type="button"
      variant="outline"
      size="icon"
      title="Làm lại"
      disabled={disabled || !canRedo}
      onClick={onRedo}
    >
      <Redo2 className="size-4" />
    </Button>
  </div>
);
