import type { ReactNode } from "react";

interface PageHeaderProps {
  title: string;
  description?: string;
  action?: ReactNode;
}

export const PageHeader = ({ title, description, action }: PageHeaderProps) => {
  return (
    <div className="mb-6 flex flex-col gap-4 border-b border-border pb-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 className="font-heading text-2xl font-semibold tracking-tight text-foreground uppercase">
          {title}
        </h1>
        {description && (
          <p className="mt-0.5 text-sm text-muted-foreground">{description}</p>
        )}
      </div>
      {action}
    </div>
  );
};
