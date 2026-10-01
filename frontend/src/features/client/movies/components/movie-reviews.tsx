import { useState } from "react";
import { MessageSquareText } from "lucide-react";
import { EmptyState } from "@/components/common/empty-state";
import { InfiniteScrollTrigger } from "@/components/common/infinite-scroll-trigger";
import { ReviewForm } from "@/features/client/movies/components/review-form";
import { ReviewItem } from "@/features/client/movies/components/review-item";
import { useCanReviewStatus, useCreateReview, useDeleteMyReview, useMovieReviews, useUpdateReview } from "@/features/client/movies/hooks/use-movies";
import { useAuthUser } from "@/stores/auth-store";

interface MovieReviewsProps {
  movieId: number;
}

export const MovieReviews = ({ movieId }: MovieReviewsProps) => {
  const user = useAuthUser();
  const [editingId, setEditingId] = useState<number | null>(null);

  const { data, isLoading, hasNextPage, isFetchingNextPage, fetchNextPage } = useMovieReviews({ movieId });
  const { data: canReviewData } = useCanReviewStatus(movieId, !!user);
  const createReview = useCreateReview(movieId);
  const updateReview = useUpdateReview(movieId);
  const deleteReview = useDeleteMyReview(movieId);

  const reviews = data?.reviews ?? [];
  const editingReview = reviews.find((r) => r.reviewId === editingId);

  return (
    <div>
      <h2 className="font-heading text-2xl font-semibold tracking-tight text-foreground uppercase md:text-3xl">Đánh giá & Bình luận</h2>

      {user && canReviewData?.canReview && !editingId && (
        <div className="mt-4">
          <ReviewForm isSubmitting={createReview.isPending} onSubmit={(payload) => createReview.mutate(payload)} />
        </div>
      )}

      {editingReview && (
        <div className="mt-4">
          <ReviewForm
            initialRating={editingReview.rating}
            initialComment={editingReview.comment ?? ""}
            isSubmitting={updateReview.isPending}
            submitLabel="Cập nhật"
            onCancel={() => setEditingId(null)}
            onSubmit={(payload) => updateReview.mutate({ reviewId: editingReview.reviewId, body: payload }, { onSuccess: () => setEditingId(null) })}
          />
        </div>
      )}

      <div className="mt-6">
        {isLoading ? (
          <p className="text-sm text-muted-foreground">Đang tải đánh giá...</p>
        ) : reviews.length === 0 ? (
          <EmptyState icon={MessageSquareText} title="Chưa có đánh giá nào" description="Hãy là người đầu tiên chia sẻ cảm nhận về bộ phim này." />
        ) : (
          <div>
            {reviews.map((review) => (
              <ReviewItem key={review.reviewId} review={review} isOwner={user?.userId === review.userId} onEdit={() => setEditingId(review.reviewId)} onDelete={() => deleteReview.mutate(review.reviewId)} />
            ))}
            <InfiniteScrollTrigger hasNextPage={hasNextPage} isFetchingNextPage={isFetchingNextPage} fetchNextPage={fetchNextPage} />
          </div>
        )}
      </div>
    </div>
  );
}