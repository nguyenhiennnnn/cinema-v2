import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { cn, getInitials } from "@/lib/utils";

const SIZE_MAP = {
  sm: "size-8",
  md: "size-9",
  lg: "size-12",
} as const;

interface UserAvatarProps {
  src?: string | null;
  name?: string | null;
  size?: keyof typeof SIZE_MAP;
  className?: string;
}

export const UserAvatar = ({
  src,
  name,
  size = "md",
  className,
}: UserAvatarProps) => {
  return (
    <Avatar className={cn(SIZE_MAP[size], "border border-border", className)}>
      <AvatarImage src={src ?? undefined} alt={name ?? "avatar"} />
      <AvatarFallback className="text-xs font-medium">
        {getInitials(name)}
      </AvatarFallback>
    </Avatar>
  );
};
