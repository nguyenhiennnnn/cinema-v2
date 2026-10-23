import * as React from "react"

import { cn } from "@/lib/utils"

function Textarea({ className, ...props }: React.ComponentProps<"textarea">) {
  return (
    <textarea
      data-slot="textarea"
      className={cn(
        "flex field-sizing-content min-h-16 w-full bg-secondary px-3 py-2 text-base transition-colors outline-none border-0 border-b-2 border-input placeholder:text-muted-foreground focus-visible:border-b-primary disabled:cursor-not-allowed disabled:bg-input/50 disabled:opacity-50 aria-invalid:border-b-destructive md:text-sm",
        className
      )}
      {...props}
    />
  )
}

export { Textarea }
