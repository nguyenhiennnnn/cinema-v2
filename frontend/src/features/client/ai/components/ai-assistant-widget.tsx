import { useEffect, useRef, useState, type CSSProperties } from "react";
import { Sparkles, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { useAiChat } from "../hooks/use-ai-chat";
import type { AiCard, ChatMessageDto } from "../types";
import { AiChatMessageBubble } from "./ai-chat-message-bubble";
import { AiChatCards } from "./ai-chat-cards";
import { AiChatInput } from "./ai-chat-input";
import { AiTypingIndicator } from "./ai-typing-indicator";

interface AiMessage {
  id: string;
  role: "user" | "assistant";
  content: string;
  cards?: AiCard[];
}

const SUGGESTIONS = [
  "Phim nào đang chiếu hôm nay?",
  "Lịch chiếu cuối tuần này",
  "Suất tối nay còn ghế không?",
  "Vé tôi đã đặt",
];

const GREETING: AiMessage = {
  id: "greeting",
  role: "assistant",
  content:
    "Chào bạn! Mình là **Trợ lý Perfo**, có thể giúp bạn tìm phim, lịch chiếu, ưu đãi... Bạn cần hỗ trợ gì ạ?",
};

export const AiAssistantWidget = () => {
  const [open, setOpen] = useState(false);
  const [messages, setMessages] = useState<AiMessage[]>([GREETING]);
  const scrollRef = useRef<HTMLDivElement>(null);
  const chat = useAiChat();

  useEffect(() => {
    scrollRef.current?.scrollTo({ top: scrollRef.current.scrollHeight, behavior: "smooth" });
  }, [messages, chat.isPending]);

  const handleSend = (text: string) => {
    const trimmed = text.trim();
    if (!trimmed || chat.isPending) return;

    const userMessage: AiMessage = { id: crypto.randomUUID(), role: "user", content: trimmed };
    const history: ChatMessageDto[] = messages
      .filter((m) => m.id !== "greeting")
      .map((m) => ({ role: m.role, content: m.content }));

    setMessages((prev) => [...prev, userMessage]);

    chat.mutate(
      { message: trimmed, history },
      {
        onSuccess: (data) => {
          setMessages((prev) => [
            ...prev,
            {
              id: crypto.randomUUID(),
              role: "assistant",
              content: data.reply,
              cards: data.cards ?? [],
            },
          ]);
        },
        onError: () => {
          setMessages((prev) => [
            ...prev,
            {
              id: crypto.randomUUID(),
              role: "assistant",
              content: "Xin lỗi, hiện mình chưa thể phản hồi. Bạn vui lòng thử lại sau ít phút nhé.",
            },
          ]);
        },
      },
    );
  };

  return (
    <>
      {/* Panel */}
      <div
        className={cn(
          "fixed bottom-24 right-4 z-50 flex w-95 max-w-[calc(100vw-2rem)] origin-bottom-right flex-col overflow-hidden border border-border bg-card transition-all duration-200 sm:right-6",
          open
            ? "h-140 max-h-[calc(100vh-8rem)] scale-100 opacity-100"
            : "pointer-events-none h-0 scale-95 opacity-0",
        )}
      >
        {/* Header */}
        <div className="flex items-center justify-between gap-2 bg-primary px-4 py-3 text-primary-foreground">
          <div className="flex items-center gap-2.5">
            <div className="flex size-9 shrink-0 items-center justify-center border border-primary-foreground/30 bg-primary-foreground/10">
              <Sparkles className="size-4" />
            </div>
            <div>
              <p className="font-heading text-sm font-semibold tracking-tight">Trợ lý Perfo</p>
              <p className="flex items-center gap-1 text-xs text-primary-foreground/70">
                <span className="size-1.5 inline-block bg-success" />
                Luôn sẵn sàng hỗ trợ
              </p>
            </div>
          </div>
          <button
            type="button"
            onClick={() => setOpen(false)}
            className="flex size-8 items-center justify-center text-primary-foreground/70 transition-colors hover:bg-primary-foreground/10 hover:text-primary-foreground"
            aria-label="Đóng trợ lý"
          >
            <X className="size-4" />
          </button>
        </div>
        <div className="notch-x perf-h shrink-0" style={{ "--notch-bg": "var(--card)" } as CSSProperties} />

        {/* Messages */}
        <div ref={scrollRef} className="flex-1 space-y-4 overflow-y-auto px-4 py-4">
          {messages.map((m) => (
            <div key={m.id}>
              <AiChatMessageBubble role={m.role} content={m.content} />
              {m.cards && m.cards.length > 0 && (
                <AiChatCards cards={m.cards} onNavigate={() => setOpen(false)} />
              )}
            </div>
          ))}
          {chat.isPending && <AiTypingIndicator />}

          {messages.length === 1 && (
            <div className="flex flex-wrap gap-2 pt-1">
              {SUGGESTIONS.map((s) => (
                <button
                  key={s}
                  type="button"
                  onClick={() => handleSend(s)}
                  className="border border-border bg-secondary px-3 py-1.5 text-xs font-medium text-secondary-foreground transition-colors hover:border-primary hover:text-primary"
                >
                  {s}
                </button>
              ))}
            </div>
          )}
        </div>

        <AiChatInput onSend={handleSend} disabled={chat.isPending} />
      </div>

      {/* Trigger button */}
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        className="cut-md fixed bottom-6 right-4 z-50 flex size-14 items-center justify-center border border-primary bg-primary text-primary-foreground transition-colors hover:bg-primary/90 sm:right-6"
        aria-label={open ? "Đóng trợ lý AI" : "Mở trợ lý AI"}
      >
        {open ? <X className="size-6" /> : <Sparkles className="size-6" />}
      </button>
    </>
  );
};
