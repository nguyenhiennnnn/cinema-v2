import { useCallback } from "react";
import type { FieldPath, FieldValues, UseFormReturn } from "react-hook-form";
import { getFieldErrors } from "@/lib/error";

export function useServerFieldErrors<T extends FieldValues>(
  form: Pick<UseFormReturn<T>, "setError">,
) {
  const { setError } = form;

  return useCallback(
    (error: unknown): boolean => {
      const entries = Object.entries(getFieldErrors(error)).filter(
        ([field, message]) => field !== "" && message !== "",
      );
      entries.forEach(([field, message], index) => {
        setError(
          field as FieldPath<T>,
          { type: "server", message },
          { shouldFocus: index === 0 },
        );
      });
      return entries.length > 0;
    },
    [setError],
  );
}
