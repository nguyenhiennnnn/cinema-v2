import { Star } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { InfiniteScrollTrigger } from "@/components/common/infinite-scroll-trigger";
import { AccountReviewCard } from "@/features/client/account/components/account-review-card";
import { useMyReviews } from "@/features/client/account/hooks/use-account";

const AccountReviewsPage = () => {
  const { data, isLoading, hasNextPage, isFetchingNextPage, fetchNextPage } = useMyReviews({ limit: 10 });
  const reviews = data?.reviews ?? [];

  return (
    <div>
      <h1 className="font-heading text-xl font-semibold text-foreground">Đánh giá của tôi</h1>

      <div className="mt-5 space-y-3">
        {isLoading ? (
          Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-24 w-full rounded-xl" />
          ))
        ) : reviews.length === 0 ? (
          <EmptyState icon={Star} title="Bạn chưa có đánh giá nào" description="Hãy chia sẻ cảm nhận sau khi xem phim tại rạp." />
        ) : (
          <>
            {reviews.map((review) => (
              <AccountReviewCard key={review.reviewId} review={review} />
            ))}
            <InfiniteScrollTrigger
              hasNextPage={hasNextPage}
              isFetchingNextPage={isFetchingNextPage}
              fetchNextPage={fetchNextPage}
            />
          </>
        )}
      </div>
    </div>
  );
};

export default AccountReviewsPage;