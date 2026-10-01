import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import { cn } from "@/lib/utils";
import { useSheetClose, useSheetState } from "@/stores/sheet-store";

export const GlobalSheet = () => {
  const { isOpen, title, description, content, side, className } =
    useSheetState();
  const close = useSheetClose();

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && close()}>
      <SheetContent
        side={side}
        className={cn("flex flex-col p-0 gap-0", className)}
      >
        {(title || description) && (
          <SheetHeader className="shrink-0 px-6 pt-6 pb-4 border-b border-border/40">
            {title && <SheetTitle>{title}</SheetTitle>}
            {description && <SheetDescription>{description}</SheetDescription>}
          </SheetHeader>
        )}
        <div className="flex-1 overflow-y-auto p-6">{content}</div>
      </SheetContent>
    </Sheet>
  );
};