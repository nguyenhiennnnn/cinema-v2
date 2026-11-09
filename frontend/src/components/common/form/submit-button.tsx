import type { ComponentProps, ReactNode } from "react";
import { Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";

interface SubmitButtonProps
  extends Omit<ComponentProps<typeof Button>, "type" | "children"> {
  isPending?: boolean;
  label: string;
  pendingLabel?: string;
  icon?: ReactNode;
}

export const SubmitButton = ({
  isPending = false,
  label,
  pendingLabel,
  icon,
  disabled,
  ...props
}: SubmitButtonProps) => (
  <Button type="submit" disabled={isPending || disabled} {...props}>
    {isPending ? <Loader2 className="size-4 animate-spin" /> : icon}
    {isPending ? (pendingLabel ?? label) : label}
  </Button>
);
