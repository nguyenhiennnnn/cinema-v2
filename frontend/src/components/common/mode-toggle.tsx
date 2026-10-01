import { Moon, Sun } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { useTheme } from "@/app/providers/theme-provider";

export const ModeToggle = () => {
  const { theme, setTheme } = useTheme();

  const isDark =
    theme === "dark" ||
    (theme === "system" &&
      window.matchMedia("(prefers-color-scheme: dark)").matches);

  return (
    <Tooltip>
      <TooltipTrigger
        render={
          <Button
            variant="outline"
            size="icon"
            onClick={() => setTheme(isDark ? "light" : "dark")}
          >
            <Sun className="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90" />
            <Moon className="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0" />
            <span className="sr-only">Chuyển giao diện</span>
          </Button>
        }
      />
      <TooltipContent>{isDark ? "Chuyển sang giao diện sáng" : "Chuyển sang giao diện tối"}</TooltipContent>
    </Tooltip>
  );
};
