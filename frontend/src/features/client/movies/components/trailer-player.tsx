import { toEmbedUrl } from "../utils/embed-url";

interface TrailerPlayerProps {
  trailerUrl: string;
  title: string;
}

export const TrailerPlayer = ({ trailerUrl, title }: TrailerPlayerProps) => (
  <div className="aspect-video w-full">
    <iframe
      src={toEmbedUrl(trailerUrl)}
      title={title}
      allow="autoplay; encrypted-media; picture-in-picture"
      allowFullScreen
      className="h-full w-full"
    />
  </div>
);