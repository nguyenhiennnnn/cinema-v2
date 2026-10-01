import { Bot } from "lucide-react";

export const AiTypingIndicator = () => {
  return (
    <div className="flex items-end gap-2">
      <div className="flex size-7 shrink-0 items-center justify-center border border-primary/30 bg-primary/15 text-primary">
        <Bot className="size-3.5" />
      </div>
      <div className="flex items-center gap-1 border border-border bg-card px-4 py-3">
        <span className="size-1.5 animate-bounce rounded-full bg-muted-foreground [animation-delay:-0.3s]" />
        <span className="size-1.5 animate-bounce rounded-full bg-muted-foreground [animation-delay:-0.15s]" />
        <span className="size-1.5 animate-bounce rounded-full bg-muted-foreground" />
      </div>
    </div>
  );
};