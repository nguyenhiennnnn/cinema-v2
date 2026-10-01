import { Button } from "@/components/ui/button";
import { RatingStars } from "@/components/common/rating-stars";
import { UserAvatar } from "@/components/common/user-avatar";
import { formatDate } from "@/lib/formatters";
import type { ReviewDto } from "@/features/client/movies/types";

interface ReviewItemProps {
  review: ReviewDto;
  isOwner?: boolean;
  onEdit?: () => void;
  onDelete?: () => void;
}

export const ReviewItem = ({ review, isOwner, onEdit, onDelete }: ReviewItemProps) => {
  return (
    <div className="flex gap-3 border-b border-border/60 py-4 last:border-none">
      <UserAvatar src={review.userAvatar} name={review.userName} size="md" />

      <div className="flex-1">
        <div className="flex flex-wrap items-center justify-between gap-2">
          <div>
            <p className="text-sm font-semibold text-foreground">{review.userName}</p>
            <div className="mt-0.5 flex items-center gap-2">
              <RatingStars value={review.rating} size="sm" />
              <span className="text-xs text-muted-foreground">{formatDate(review.createdAt)}</span>
            </div>
          </div>

          {isOwner && (
            <div className="flex gap-1">
              <Button variant="ghost" size="sm" onClick={onEdit}>
                Sửa
              </Button>
              <Button variant="ghost" size="sm" className="text-destructive hover:text-destructive" onClick={onDelete}>
                Xóa
              </Button>
            </div>
          )}
        </div>

        {review.comment && <p className="mt-2 text-sm leading-relaxed text-foreground">{review.comment}</p>}
      </div>
    </div>
  );
}