import type { LucideIcon } from "lucide-react";
import {
  Armchair,
  Award,
  Briefcase,
  Clapperboard,
  Globe2,
  MonitorPlay,
  ShieldAlert,
  UserRound,
  Video,
} from "lucide-react";

import { PageHeader } from "@/components/common/page-header";

import { useAdminGenres } from "../hooks/use-admin-genre";
import { useAdminLanguages } from "../hooks/use-admin-language";
import { useAdminAgeRatings } from "../hooks/use-admin-age-rating";
import { useAdminSeatTypes } from "../hooks/use-admin-seat-type";
import { useAdminScreenTypes } from "../hooks/use-admin-screen-type";
import { useAdminMembershipTiers } from "../hooks/use-admin-membership-tier";
import { useAdminPositions } from "../hooks/use-admin-position";
import { useAdminActors } from "../hooks/use-admin-actor";
import { useAdminDirectors } from "../hooks/use-admin-director";
import { CatalogCard } from "../components/catalog-card";

interface CatalogCardConfig {
  key: string;
  to: string;
  icon: LucideIcon;
  title: string;
  description: string;
  count?: number;
  isLoading: boolean;
  accent: string;
}

const COUNT_ONLY_PARAMS = { page: 1, pageSize: 1 };

const CatalogHubPage = () => {
  const genres = useAdminGenres(COUNT_ONLY_PARAMS);
  const languages = useAdminLanguages(COUNT_ONLY_PARAMS);
  const ageRatings = useAdminAgeRatings(COUNT_ONLY_PARAMS);
  const seatTypes = useAdminSeatTypes(COUNT_ONLY_PARAMS);
  const screenTypes = useAdminScreenTypes(COUNT_ONLY_PARAMS);
  const membershipTiers = useAdminMembershipTiers(COUNT_ONLY_PARAMS);
  const positions = useAdminPositions(COUNT_ONLY_PARAMS);
  const actors = useAdminActors(COUNT_ONLY_PARAMS);
  const directors = useAdminDirectors(COUNT_ONLY_PARAMS);

  const cards: CatalogCardConfig[] = [
    {
      key: "genres",
      to: "/admin/catalog/genres",
      icon: Clapperboard,
      title: "Thể loại phim",
      description: "Hành động, Kinh dị, Hoạt hình...",
      count: genres.data?.totalItems,
      isLoading: genres.isLoading,
      accent: "text-primary bg-primary/10",
    },
    {
      key: "languages",
      to: "/admin/catalog/languages",
      icon: Globe2,
      title: "Ngôn ngữ",
      description: "Ngôn ngữ & hình thức dịch phim.",
      count: languages.data?.totalItems,
      isLoading: languages.isLoading,
      accent: "text-chart-3 bg-chart-3/10",
    },
    {
      key: "age-ratings",
      to: "/admin/catalog/age-ratings",
      icon: ShieldAlert,
      title: "Phân loại độ tuổi",
      description: "P, K, T13, T16, T18, C.",
      count: ageRatings.data?.totalItems,
      isLoading: ageRatings.isLoading,
      accent: "text-destructive bg-destructive/10",
    },
    {
      key: "seat-types",
      to: "/admin/catalog/seat-types",
      icon: Armchair,
      title: "Loại ghế",
      description: "Thường, VIP, Sweetbox và hệ số phụ thu.",
      count: seatTypes.data?.totalItems,
      isLoading: seatTypes.isLoading,
      accent: "text-accent bg-accent/10",
    },
    {
      key: "screen-types",
      to: "/admin/catalog/screen-types",
      icon: MonitorPlay,
      title: "Loại màn hình",
      description: "2D, 3D, IMAX Laser, ScreenX.",
      count: screenTypes.data?.totalItems,
      isLoading: screenTypes.isLoading,
      accent: "text-couple bg-couple/10",
    },
    {
      key: "membership-tiers",
      to: "/admin/catalog/membership-tiers",
      icon: Award,
      title: "Hạng thành viên",
      description: "Bronze, Silver, Gold, Platinum.",
      count: membershipTiers.data?.totalItems,
      isLoading: membershipTiers.isLoading,
      accent: "text-warning bg-warning/10",
    },
    {
      key: "positions",
      to: "/admin/catalog/positions",
      icon: Briefcase,
      title: "Chức vụ",
      description: "Danh sách chức vụ nhân viên rạp.",
      count: positions.data?.totalItems,
      isLoading: positions.isLoading,
      accent: "text-success bg-success/10",
    },
    {
      key: "actors",
      to: "/admin/catalog/actors",
      icon: UserRound,
      title: "Diễn viên",
      description: "Hồ sơ diễn viên & ảnh đại diện.",
      count: actors.data?.totalItems,
      isLoading: actors.isLoading,
      accent: "text-couple bg-couple/10",
    },
    {
      key: "directors",
      to: "/admin/catalog/directors",
      icon: Video,
      title: "Đạo diễn",
      description: "Hồ sơ đạo diễn & tiểu sử.",
      count: directors.data?.totalItems,
      isLoading: directors.isLoading,
      accent: "text-chart-3 bg-chart-3/10",
    },
  ];

  return (
    <div>
      <PageHeader
        title="Trung tâm Danh mục"
        description="Quản lý các bảng danh mục dữ liệu chuẩn của hệ thống rạp chiếu phim."
      />

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
        {cards.map((card) => (
          <CatalogCard {...card} />
        ))}
      </div>
    </div>
  );
};

export default CatalogHubPage;
