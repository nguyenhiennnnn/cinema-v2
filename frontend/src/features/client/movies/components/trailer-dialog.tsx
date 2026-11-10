import { Dialog, DialogContent } from "@/components/ui/dialog";
import { toEmbedUrl } from "../utils/embed-url";

interface TrailerDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  trailerUrl: string;
  title: string;
}

export const TrailerDialog = ({ open, onOpenChange, trailerUrl, title }: TrailerDialogProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent size="xl" className="overflow-hidden border-none bg-scrim p-0">
        <div className="aspect-video w-full">
          {open && (
            <iframe
              src={toEmbedUrl(trailerUrl)}
              title={title}
              allow="autoplay; encrypted-media; picture-in-picture"
              allowFullScreen
              className="h-full w-full"
            />
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}