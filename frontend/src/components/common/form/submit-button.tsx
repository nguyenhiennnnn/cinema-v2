import type { ComponentProps, ReactNode } from "react";
import { Spinner } from "@/components/common/spinner";

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
    {isPending ? <Spinner size="sm" decorative className="text-current" /> : icon}
    {isPending ? (pendingLabel ?? label) : label}
  </Button>
);
