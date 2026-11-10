import { Link } from "react-router";
import { ChevronRight, Film, Ticket } from "lucide-react";
import type { AiCard } from "@/features/client/ai/types";

interface AiChatCardsProps {
  cards: AiCard[];
  onNavigate?: () => void;
}

const ACTION_LABEL: Record<AiCard["type"], string> = {
  showtime: "Đặt vé",
  booking: "Xem vé",
};

export const AiChatCards = ({ cards, onNavigate }: AiChatCardsProps) => {
  if (cards.length === 0) return null;

  return (
    <div className="mt-2 space-y-2 pl-9">
      {cards.map((card) => (
        <Link
          key={`${card.type}-${card.link}`}
          to={card.link}
          onClick={onNavigate}
          className="group flex items-stretch gap-3 border border-border bg-card p-2 transition-colors hover:border-primary"
        >
          <div className="aspect-2/3 w-12 shrink-0 overflow-hidden bg-muted">
            {card.imageUrl ? (
              <img
                src={card.imageUrl}
                alt={card.title}
                loading="lazy"
                className="size-full object-cover"
              />
            ) : (
              <div className="flex size-full items-center justify-center text-muted-foreground">
                {card.type === "booking" ? (
                  <Ticket className="size-4" />
                ) : (
                  <Film className="size-4" />
                )}
              </div>
            )}
          </div>

          <div className="flex min-w-0 flex-1 flex-col justify-between py-0.5">
            <div className="min-w-0">
              <p className="line-clamp-1 font-heading text-sm font-semibold text-foreground">
                {card.title}
              </p>
              {card.subtitle && (
                <p className="line-clamp-1 text-xs text-muted-foreground">{card.subtitle}</p>
              )}
              {card.detail && (
                <p className="line-clamp-1 text-xs font-medium tabular-nums text-foreground">
                  {card.detail}
                </p>
              )}
            </div>
            <span
              className="mt-1 inline-flex items-center gap-0.5 text-xs font-medium text-primary"
            >
              {ACTION_LABEL[card.type]}
              <ChevronRight className="size-3.5" />
            </span>
          </div>
        </Link>
      ))}
    </div>
  );
};
