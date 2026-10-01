import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";

export const HeaderSearch = () => (
  <div className="hidden max-w-xs flex-1 md:block">
    <div className="relative">
      <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
      <Input placeholder="Tìm kiếm phim, diễn viên..." className="pl-9" />
      <kbd className="absolute top-1/2 right-2 -translate-y-1/2 rounded border border-border px-1.5 py-0.5 text-[10px] text-muted-foreground">
        Ctrl K
      </kbd>
    </div>
  </div>
);