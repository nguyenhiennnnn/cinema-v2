import { useState } from "react";
import { Link } from "react-router";
import { Ticket } from "lucide-react";
import { Button } from "@/components/ui/button";
import { RatingStars } from "@/components/common/rating-stars";
import { ReviewForm } from "@/features/client/movies/components/review-form";
import { useDeleteMyReview, useUpdateReview } from "@/features/client/movies/hooks/use-movies";
import { formatDate } from "@/lib/formatters";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import type { UserMyReviewDto } from "@/features/client/account/types";

interface AccountReviewCardProps {
  review: UserMyReviewDto;
}

export const AccountReviewCard = ({ review }: AccountReviewCardProps) => {
  const [editing, setEditing] = useState(false);
  const updateReview = useUpdateReview(review.movieId);
  const deleteReview = useDeleteMyReview(review.movieId);
  const openAlert = useAlertDialogOpen();

  const handleDelete = () => {
    openAlert({
      title: "Xóa đánh giá",
      description: "Bạn có chắc muốn xóa đánh giá này?",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: () => deleteReview.mutate(review.reviewId),
    });
  };

  return (
    <div className="bg-card p-4">
      <div className="flex gap-3">
        <Link to={`/movies/${review.movieId}`} className="aspect-2/3 w-16 shrink-0 overflow-hidden bg-muted">
          {review.moviePosterUrl ? (
            <img src={review.moviePosterUrl} alt={review.movieTitle} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
              <Ticket className="size-5" />
            </div>
          )}
        </Link>

        <div className="flex-1">
          <div className="flex flex-wrap items-start justify-between gap-2">
            <div>
              <Link to={`/movies/${review.movieId}`} className="font-heading text-sm font-semibold text-foreground hover:text-primary">
                {review.movieTitle}
              </Link>
              <div className="mt-1 flex items-center gap-2">
                <RatingStars value={review.rating} size="sm" />
                <span className="text-xs text-muted-foreground">{formatDate(review.createdAt)}</span>
              </div>
            </div>
            {!editing && (
              <div className="flex gap-1">
                <Button variant="ghost" size="sm" onClick={() => setEditing(true)}>
                  Sửa
                </Button>
                <Button variant="ghost" size="sm" className="text-destructive hover:text-destructive" onClick={handleDelete}>
                  Xóa
                </Button>
              </div>
            )}
          </div>
          {!editing && review.comment && <p className="mt-2 text-sm leading-relaxed text-foreground">{review.comment}</p>}
        </div>
      </div>

      {editing && (
        <div className="mt-4">
          <ReviewForm
            initialRating={review.rating}
            initialComment={review.comment ?? ""}
            isSubmitting={updateReview.isPending}
            submitLabel="Cập nhật"
            onCancel={() => setEditing(false)}
            onSubmit={(payload) => updateReview.mutate({ reviewId: review.reviewId, body: payload }, { onSuccess: () => setEditing(false) })}
          />
        </div>
      )}
    </div>
  );
}
