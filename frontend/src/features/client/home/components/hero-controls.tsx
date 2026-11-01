import { ChevronLeft, ChevronRight, Pause, Play } from "lucide-react";

const controlClass =
  "cut-sm flex size-11 items-center justify-center bg-scrim-foreground/15 text-scrim-foreground transition-colors duration-150 hover:bg-scrim-foreground/25";

interface HeroControlsProps {
  index: number;
  count: number;
  canAutoplay: boolean;
  userPaused: boolean;
  onPrev: () => void;
  onNext: () => void;
  onTogglePause: () => void;
}

export const HeroControls = ({
  index,
  count,
  canAutoplay,
  userPaused,
  onPrev,
  onNext,
  onTogglePause,
}: HeroControlsProps) => (
  <div className="flex items-center gap-2">
    <button type="button" onClick={onPrev} className={controlClass} aria-label="Phim trước">
      <ChevronLeft className="size-5" />
    </button>
    <button type="button" onClick={onNext} className={controlClass} aria-label="Phim tiếp theo">
      <ChevronRight className="size-5" />
    </button>
    {canAutoplay && (
      <button
        type="button"
        onClick={onTogglePause}
        className={controlClass}
        aria-label={userPaused ? "Tiếp tục tự chuyển phim" : "Tạm dừng tự chuyển phim"}
      >
        {userPaused ? <Play className="size-5" /> : <Pause className="size-5" />}
      </button>
    )}
    <span className="t-code ml-2 text-sm text-scrim-foreground/70" aria-hidden="true">
      {index + 1} / {count}
    </span>
  </div>
);
