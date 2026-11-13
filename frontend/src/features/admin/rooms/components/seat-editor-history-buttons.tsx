import type { ReactNode } from "react";
import { Redo2, Undo2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { SeatEditorKbd } from "./seat-editor-kbd";

interface SeatEditorHistoryButtonsProps {
  disabled: boolean;
  canUndo: boolean;
  canRedo: boolean;
  onUndo: () => void;
  onRedo: () => void;
}

interface HistoryButtonProps {
  label: string;
  shortcut: string;
  disabled: boolean;
  onClick: () => void;
  children: ReactNode;
}

const HistoryButton = ({ label, shortcut, disabled, onClick, children }: HistoryButtonProps) => (
  <Tooltip>
    <TooltipTrigger render={<span className="inline-flex" />}>
      <Button
        type="button"
        variant="outline"
        size="icon"
        aria-label={`${label} (${shortcut})`}
        disabled={disabled}
        onClick={onClick}
        className="disabled:opacity-30"
      >
        {children}
      </Button>
    </TooltipTrigger>
    <TooltipContent>
      {label}
      <SeatEditorKbd>{shortcut}</SeatEditorKbd>
    </TooltipContent>
  </Tooltip>
);

export const SeatEditorHistoryButtons = ({
  disabled,
  canUndo,
  canRedo,
  onUndo,
  onRedo,
}: SeatEditorHistoryButtonsProps) => (
  <div className="flex items-center gap-1">
    <HistoryButton label="Hoàn tác" shortcut="Ctrl+Z" disabled={disabled || !canUndo} onClick={onUndo}>
      <Undo2 className="size-4" />
    </HistoryButton>
    <HistoryButton label="Làm lại" shortcut="Ctrl+Y" disabled={disabled || !canRedo} onClick={onRedo}>
      <Redo2 className="size-4" />
    </HistoryButton>
  </div>
);
