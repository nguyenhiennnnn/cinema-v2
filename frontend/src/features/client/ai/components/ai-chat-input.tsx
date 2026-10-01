import { useState, type KeyboardEvent } from "react";
import { SendHorizonal } from "lucide-react";
import { cn } from "@/lib/utils";

interface AiChatInputProps {
  onSend: (text: string) => void;
  disabled?: boolean;
}

export const AiChatInput = ({ onSend, disabled }: AiChatInputProps) => {
  const [value, setValue] = useState("");

  const handleSubmit = () => {
    if (!value.trim() || disabled) return;
    onSend(value);
    setValue("");
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSubmit();
    }
  };

  return (
    <div className="flex items-end gap-2 border-t border-border bg-card p-3">
      <textarea
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onKeyDown={handleKeyDown}
        disabled={disabled}
        rows={1}
        placeholder="Nhập câu hỏi của bạn..."
        className="max-h-24 flex-1 resize-none rounded-lg border border-input bg-background px-3 py-2.5 text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-primary focus-visible:outline-none focus-visible:ring-3 focus-visible:ring-ring/30 disabled:opacity-60"
      />
      <button
        type="button"
        onClick={handleSubmit}
        disabled={disabled || !value.trim()}
        className={cn(
          "flex size-10 shrink-0 items-center justify-center rounded-lg bg-primary text-primary-foreground transition-all hover:bg-primary/90 active:scale-95",
          (disabled || !value.trim()) && "cursor-not-allowed opacity-40",
        )}
        aria-label="Gửi tin nhắn"
      >
        <SendHorizonal className="size-4" />
      </button>
    </div>
  );
};