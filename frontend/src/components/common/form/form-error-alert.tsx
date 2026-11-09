import { getDisplayMessage, hasFieldErrors } from "@/lib/error";

interface FormErrorAlertProps {
  error: unknown;
  message?: string;
  preferServerMessage?: boolean;
}

export const FormErrorAlert = ({
  error,
  message,
  preferServerMessage,
}: FormErrorAlertProps) => {
  if (!error || hasFieldErrors(error)) return null;

  return (
    <p className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive">
      {message ?? getDisplayMessage(error, { preferServerMessage })}
    </p>
  );
};
