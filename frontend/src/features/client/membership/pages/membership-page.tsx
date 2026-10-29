import { Skeleton } from "@/components/ui/skeleton";
import { TierCard } from "../components/tier-card";
import { TierCompareTable } from "../components/tier-compare-table";
import { MembershipFaq } from "../components/membership-faq";
import { useMembershipTiers } from "../../catalog/hooks/use-catalog";
import { Seo } from "@/components/common/seo";
import { BRAND } from "@/config/brand";

const MembershipPage = () => {
  const { data: tiers, isLoading, isError } = useMembershipTiers();

  const sortedTiers = [...(tiers ?? [])].sort(
    (a, b) => a.minPoints - b.minPoints,
  );

  return (
    <div className="mx-auto max-w-6xl px-4 py-10 md:py-14">
      <Seo
        title={`Chương trình thành viên ${BRAND.name} Rewards`}
        description={`Tích điểm mỗi lần đặt vé và nhận quyền lợi độc quyền theo hạng thành viên ${BRAND.name} Rewards: Bronze, Silver, Gold, Platinum.`}
        canonical="/membership"
      />
      <section className="mb-10 border border-border/60 bg-card p-6 text-center md:p-10">
        <p className="mb-3 text-sm font-medium text-accent">{BRAND.name} Rewards</p>
        <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground md:text-4xl">
          Chương trình Khách hàng Thân thiết {BRAND.name}
        </h1>
        <p className="mx-auto mt-3 max-w-xl text-sm text-muted-foreground md:text-base">
          Mỗi <span className="font-semibold text-foreground">10.000 VNĐ</span>{" "}
          chi tiêu = <span className="font-semibold text-foreground">1</span>{" "}
          điểm thưởng tích lũy. Càng xem nhiều, quyền lợi càng lớn.
        </p>
      </section>

      <section className="mb-14">
        <h2 className="mb-5 font-heading text-2xl font-semibold tracking-tight text-foreground md:text-3xl">
          Các hạng thẻ hội viên
        </h2>

        {isError && (
          <p className="border border-destructive/30 bg-destructive/10 p-4 text-sm text-destructive">
            Không thể tải dữ liệu hạng thẻ. Vui lòng thử lại sau.
          </p>
        )}

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {isLoading &&
            Array.from({ length: 4 }).map((_, i) => (
              <Skeleton key={i} className="h-56 w-full" />
            ))}

          {!isLoading &&
            sortedTiers.map((tier, index) => (
              <TierCard key={tier.tierId} tier={tier} index={index} />
            ))}
        </div>
      </section>

      {!isLoading && sortedTiers.length > 0 && (
        <section className="mb-14">
          <h2 className="mb-5 font-heading text-2xl font-semibold tracking-tight text-foreground md:text-3xl">
            So sánh quyền lợi chi tiết
          </h2>
          <TierCompareTable tiers={sortedTiers} />
        </section>
      )}

      <section>
        <h2 className="mb-5 font-heading text-2xl font-semibold tracking-tight text-foreground md:text-3xl">
          Câu hỏi thường gặp
        </h2>
        <MembershipFaq />
      </section>
    </div>
  );
};

export default MembershipPage;
