import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { SelectField } from "@/components/common/select-field";
import { useGenres, useLanguages, useAgeRatings } from "@/features/client/catalog/hooks/use-catalog";
import { cn } from "@/lib/utils";
import { MovieStatus } from "@/types/enum";
import type { useClientMovieParams } from "@/hooks/use-query-params";

const STATUS_TABS = [
  { value: null, label: "Tất cả" },
  { value: MovieStatus.NowShowing, label: "Đang chiếu" },
  { value: MovieStatus.ComingSoon, label: "Sắp chiếu" },
] as const;

type ClientMovieParams = ReturnType<typeof useClientMovieParams>[0];
type SetClientMovieParams = ReturnType<typeof useClientMovieParams>[1];

interface MoviesFilterBarProps {
  params: ClientMovieParams;
  setParams: SetClientMovieParams;
}

export const MoviesFilterBar = ({ params, setParams }: MoviesFilterBarProps) => {
  const { data: genres } = useGenres();
  const { data: languages } = useLanguages();
  const { data: ageRatings } = useAgeRatings();

  const hasActiveFilters = !!params.search || !!params.status || !!params.genreId || !!params.languageId || !!params.ageRatingId;

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="relative w-full sm:w-80">
          <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={params.search}
            onChange={(e) => setParams({ search: e.target.value, cursor: null })}
            placeholder="Tìm tên phim, diễn viên..."
            className="pl-9"
          />
        </div>

        <div className="flex flex-wrap items-center gap-2">
          {STATUS_TABS.map((tab) => (
            <button
              key={tab.label}
              type="button"
              onClick={() => setParams({ status: tab.value, cursor: null })}
              className={cn(
                "border px-4 py-1.5 text-sm font-medium transition-colors",
                params.status === tab.value
                  ? "border-primary bg-primary text-primary-foreground"
                  : "border-border bg-secondary text-secondary-foreground hover:border-primary/50 hover:bg-muted",
              )}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <SelectField
          value={params.genreId ?? null}
          onChange={(_, option) => setParams({ genreId: option ? option.genreId : null, cursor: null })}
          options={genres ?? []}
          getOptionValue={(g) => g.genreId}
          getOptionLabel={(g) => g.name}
          placeholder="Thể loại"
          className="w-44"
        />
        <SelectField
          value={params.languageId ?? null}
          onChange={(_, option) => setParams({ languageId: option ? option.languageId : null, cursor: null })}
          options={languages ?? []}
          getOptionValue={(l) => l.languageId}
          getOptionLabel={(l) => l.name}
          placeholder="Ngôn ngữ"
          className="w-44"
        />
        <SelectField
          value={params.ageRatingId ?? null}
          onChange={(_, option) => setParams({ ageRatingId: option ? option.ageRatingId : null, cursor: null })}
          options={ageRatings ?? []}
          getOptionValue={(a) => a.ageRatingId}
          getOptionLabel={(a) => a.code}
          placeholder="Độ tuổi"
          className="w-36"
        />
        {hasActiveFilters && (
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setParams({ search: "", status: null, genreId: null, languageId: null, ageRatingId: null, cursor: null })}
          >
            Xóa bộ lọc
          </Button>
        )}
      </div>
    </div>
  );
}