import { Link } from "react-router";
import { AlertTriangle } from "lucide-react";

import { Button } from "@/components/ui/button";

interface InvalidLinkNoticeProps {
  title: string;
  description: string;
  actionLabel: string;
  actionTo: string;
}

export const InvalidLinkNotice = ({
  title,
  description,
  actionLabel,
  actionTo,
}: InvalidLinkNoticeProps) => (
  <div className="flex flex-col items-center gap-3 text-center">
    <AlertTriangle className="size-10 text-destructive" />
    <h2 className="font-heading text-lg font-bold text-foreground">{title}</h2>
    <p className="text-sm text-muted-foreground">{description}</p>
    <Button variant="outline" render={<Link to={actionTo} />} nativeButton={false}>
      {actionLabel}
    </Button>
  </div>
);
