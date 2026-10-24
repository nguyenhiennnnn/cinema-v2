import { useEffect, useRef } from "react";
import { useNavigate, useSearchParams } from "react-router";

import { useTheme } from "@/app/providers/theme-provider";
import { toast } from "@/components/ui/toast";
import { extractApiError } from "@/lib/error";
import type { GoogleCredentialResponse } from "@/types/google-identity";

import { useGoogleLogin } from "../hooks/use-auth";

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID as
  | string
  | undefined;

interface GoogleSignInButtonProps {
  redirectTo?: string;
}

export const GoogleSignInButton = ({ redirectTo }: GoogleSignInButtonProps) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { mutate: googleLogin } = useGoogleLogin();
  const { theme } = useTheme();

  const isDark =
    theme === "dark" ||
    (theme === "system" &&
      typeof window !== "undefined" &&
      window.matchMedia("(prefers-color-scheme: dark)").matches);

  useEffect(() => {
    if (!GOOGLE_CLIENT_ID) return;

    let cancelled = false;
    let pollId: ReturnType<typeof setInterval> | undefined;

    const handleCredential = (response: GoogleCredentialResponse) => {
      googleLogin(
        { idToken: response.credential },
        {
          onSuccess: () => {
            navigate(redirectTo ?? searchParams.get("redirect") ?? "/", {
              replace: true,
            });
          },
          onError: (error) => {
            toast.add({
              title: "Đăng nhập Google thất bại",
              description: extractApiError(error).message,
              type: "error",
            });
          },
        },
      );
    };

    const renderButton = () => {
      if (cancelled || !containerRef.current || !window.google) return;

      containerRef.current.innerHTML = "";

      window.google.accounts.id.initialize({
        client_id: GOOGLE_CLIENT_ID,
        callback: handleCredential,
        cancel_on_tap_outside: true,
      });

      window.google.accounts.id.renderButton(containerRef.current, {
        type: "standard",
        theme: isDark ? "filled_black" : "outline",
        size: "large",
        text: "continue_with",
        shape: "rectangular",
        width: 336,
      });
    };

    if (window.google) {
      renderButton();
    } else {
      pollId = setInterval(() => {
        if (window.google) {
          if (pollId) clearInterval(pollId);
          renderButton();
        }
      }, 100);
    }

    return () => {
      cancelled = true;
      if (pollId) clearInterval(pollId);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isDark]);

  if (!GOOGLE_CLIENT_ID) return null;

  return <div ref={containerRef} className="flex w-full justify-center" />;
};
