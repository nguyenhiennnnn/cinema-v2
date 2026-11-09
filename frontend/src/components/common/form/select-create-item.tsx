import { CircleAlert, Loader2, Plus } from "lucide-react";
import { CommandItem } from "@/components/ui/command";

interface SelectCreateItemProps {
  term: string;
  isCreating: boolean;
  onCreate: () => void;
}

export const SelectCreateItem = ({
  term,
  isCreating,
  onCreate,
}: SelectCreateItemProps) => (
  <CommandItem
    value="__select-create__"
    disabled={isCreating}
    onSelect={onCreate}
  >
    {isCreating ? (
      <Loader2 className="mr-2 size-4 animate-spin" />
    ) : (
      <Plus className="mr-2 size-4" />
    )}
    <span className="truncate">
      {isCreating ? `Đang thêm "${term}"...` : `Thêm "${term}"`}
    </span>
  </CommandItem>
);

export const SelectCreateNote = ({ message }: { message: string | null }) =>
  message ? (
    <p
      role="alert"
      className="flex items-start gap-1.5 px-3 py-2 text-xs text-destructive"
    >
      <CircleAlert className="mt-0.5 size-3.5 shrink-0" />
      {message}
    </p>
  ) : null;
