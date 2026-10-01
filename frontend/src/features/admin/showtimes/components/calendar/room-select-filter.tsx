import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { AdminRoomDto } from "@/features/admin/rooms/types";
import { cn } from "@/lib/utils";

interface RoomSelectFilterProps {
  value: number | null;
  onChange: (value: number | null) => void;
  rooms: AdminRoomDto[];
  placeholder?: string;
  allLabel?: string;
  className?: string;
}

export const RoomSelectFilter = ({
  value,
  onChange,
  rooms,
  placeholder = "Phòng chiếu",
  allLabel = "Tất cả phòng",
  className,
}: RoomSelectFilterProps) => {
  const currentValue = value ? String(value) : "all";
  const currentLabel =
    currentValue === "all"
      ? allLabel
      : (rooms.find((r) => r.roomId === value)?.roomName ?? placeholder);

  return (
    <Select
      value={currentValue}
      onValueChange={(v) => onChange(v === "all" ? null : Number(v))}
    >
      <SelectTrigger className={cn("w-full", className)}>
        <SelectValue placeholder={placeholder}>{currentLabel}</SelectValue>
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="all">{allLabel}</SelectItem>
        {rooms.map((room) => (
          <SelectItem key={room.roomId} value={String(room.roomId)}>
            {room.roomName}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
};
