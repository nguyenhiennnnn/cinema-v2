import { useState } from "react";
import { Loader2, Send } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { RatingStars } from "@/components/common/rating-stars";

interface ReviewFormProps {
  initialRating?: number;
  initialComment?: string;
  isSubmitting?: boolean;
  submitLabel?: string;
  onSubmit: (data: { rating: number; comment: string }) => void;
  onCancel?: () => void;
}

export const ReviewForm = ({ initialRating = 0, initialComment = "", isSubmitting, submitLabel = "Gửi đánh giá", onSubmit, onCancel }: ReviewFormProps) => {
  const [rating, setRating] = useState(initialRating);
  const [comment, setComment] = useState(initialComment);

  return (
    <div className="rounded-xl border border-border/60 bg-card p-4">
      <p className="text-sm font-medium text-foreground">Đánh giá của bạn</p>
      <RatingStars value={rating} onChange={setRating} className="mt-2" />
      <Textarea value={comment} onChange={(e) => setComment(e.target.value)} placeholder="Chia sẻ cảm nhận của bạn về bộ phim..." className="mt-3" rows={3} />
      <div className="mt-3 flex justify-end gap-2">
        {onCancel && (
          <Button variant="ghost" onClick={onCancel}>
            Hủy
          </Button>
        )}
        <Button disabled={rating === 0 || isSubmitting} onClick={() => onSubmit({ rating, comment })}>
          {isSubmitting ? <Loader2 className="size-4 animate-spin" /> : <Send className="size-4" />}
          <span>{submitLabel}</span>
        </Button>
      </div>
    </div>
  );
}