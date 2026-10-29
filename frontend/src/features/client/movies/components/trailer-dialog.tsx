import { Dialog, DialogContent } from "@/components/ui/dialog";
import { YOUTUBE_EMBED_BASE_URL } from "@/config/default-constants";

interface TrailerDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  trailerUrl: string;
  title: string;
}

function toEmbedUrl(url: string) {
  const match = url.match(/(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/))([\w-]+)/);
  return match ? `${YOUTUBE_EMBED_BASE_URL}/${match[1]}?autoplay=1` : url;
}

export const TrailerDialog = ({ open, onOpenChange, trailerUrl, title }: TrailerDialogProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl overflow-hidden border-none bg-scrim p-0">
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