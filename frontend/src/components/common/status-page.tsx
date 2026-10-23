import type { LucideIcon } from "lucide-react";
import { Link } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";

export interface StatusPageAction {
  label: string;
  icon?: LucideIcon;
  variant?: "default" | "outline" | "secondary" | "ghost";
  to?: string;
  onClick?: () => void;
}

interface StatusPageProps {
  code: string;
  icon: LucideIcon;
  title: string;
  description: string;
  actions?: StatusPageAction[];
}

export const StatusPage = ({ code, icon: Icon, title, description, actions = [] }: StatusPageProps) => {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background px-4 py-16">
      <Link to="/" className="mb-10 flex items-center gap-2">
        <BrandMark className="size-8 shrink-0" />
        <BrandWordmark className="h-5 w-auto" />
      </Link>

      <div className="w-full max-w-md overflow-hidden border border-border bg-card">
        <div className="flex flex-col items-center gap-2 bg-primary px-6 py-8 text-primary-foreground">
          <Icon className="size-8" />
          <p className="font-heading text-6xl leading-none font-bold tracking-tight tabular">{code}</p>
        </div>

        <div className="notch-x perf-h" />

        <div className="flex flex-col items-center gap-2 px-6 py-6 text-center">
          <h1 className="font-heading text-lg font-semibold tracking-tight text-foreground">{title}</h1>
          <p className="max-w-xs text-sm text-muted-foreground">{description}</p>
        </div>

        {actions.length > 0 && (
          <>
            <div className="perforation" />
            <div className="flex flex-wrap items-center justify-center gap-3 px-6 py-5">
              {actions.map((action) => {
                const content = (
                  <>
                    {action.icon && <action.icon className="size-4" />}
                    {action.label}
                  </>
                );
                const className = cn(buttonVariants({ variant: action.variant ?? "default" }), "tracking-wide");

                return action.to ? (
                  <Link key={action.label} to={action.to} className={className}>
                    {content}
                  </Link>
                ) : (
                  <button key={action.label} type="button" onClick={action.onClick} className={className}>
                    {content}
                  </button>
                );
              })}
            </div>
          </>
        )}
      </div>

      <p className="mt-6 text-xs tracking-widest text-muted-foreground/70">
        Perfo · Mã lỗi {code}
      </p>
    </div>
  );
};
