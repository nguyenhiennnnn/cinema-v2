import { QueryCache, QueryClient, MutationCache } from "@tanstack/react-query";
import { extractApiError, logDevError, showApiErrorToast } from "@/lib/error";

function handleGlobalError(context: "Query" | "Mutation") {
  return (error: unknown, query: { meta?: Record<string, unknown> }) => {
    logDevError(error, context);
    if (query?.meta?.skipErrorToast) return;
    showApiErrorToast(error);
  };
}

export const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error, query) => handleGlobalError("Query")(error, query),
  }),
  mutationCache: new MutationCache({
    onError: (error, _variables, _context, mutation) =>
      handleGlobalError("Mutation")(error, mutation),
  }),
  defaultOptions: {
    queries: {
      staleTime: 30 * 1000,
      gcTime: 5 * 60 * 1000,
      retry: (failureCount, error) => {
        const { statusCode } = extractApiError(error);
        if (statusCode && statusCode >= 400 && statusCode < 500) return false;
        return failureCount < 2;
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
});
