import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import { DismissGuardContext } from "@/hooks/use-block-dialog-close";
import { cn } from "@/lib/utils";
import {
  useSheetClose,
  useSheetDismissGuard,
  useSheetState,
} from "@/stores/sheet-store";

export const GlobalSheet = () => {
  const { isOpen, title, description, content, side, className, dismissible } =
    useSheetState();
  const close = useSheetClose();
  const dismissGuard = useSheetDismissGuard();

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && dismissible && close()}>
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
        <DismissGuardContext.Provider value={dismissGuard}>
          <div className="flex-1 overflow-y-auto p-6">{content}</div>
        </DismissGuardContext.Provider>
      </SheetContent>
    </Sheet>
  );
};