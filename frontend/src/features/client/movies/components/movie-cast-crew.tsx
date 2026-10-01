import { UserAvatar } from "@/components/common/user-avatar";
import type { CastMemberDto, DirectorDto } from "@/features/client/movies/types";

interface MovieCastCrewProps {
  director?: DirectorDto | null;
  cast: CastMemberDto[];
}

export const MovieCastCrew = ({ director, cast }: MovieCastCrewProps) => {
  if (!director && cast.length === 0) return null;

  return (
    <div className="space-y-5">
      {director && (
        <div>
          <h3 className="font-heading text-base font-medium text-foreground">Đạo diễn</h3>
          <div className="mt-3 flex items-center gap-3">
            <UserAvatar src={director.avatarUrl} name={director.fullName} size="lg" />
            <span className="text-sm font-medium text-foreground">{director.fullName}</span>
          </div>
        </div>
      )}

      {cast.length > 0 && (
        <div>
          <h3 className="font-heading text-base font-medium text-foreground">Diễn viên</h3>
          <div className="mt-3 flex gap-4 overflow-x-auto pb-2">
            {cast.map((actor) => (
              <div key={actor.actorId} className="flex w-20 shrink-0 flex-col items-center gap-2 text-center">
                <UserAvatar src={actor.avatarUrl} name={actor.fullName} size="lg" />
                <div>
                  <p className="line-clamp-1 text-xs font-medium text-foreground">{actor.fullName}</p>
                  {actor.characterName && <p className="line-clamp-1 text-[10px] text-muted-foreground">{actor.characterName}</p>}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}