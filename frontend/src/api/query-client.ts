import { QueryCache, QueryClient, MutationCache } from "@tanstack/react-query";
import { extractApiError, logDevError, showApiErrorToast } from "@/lib/error";
import { QUERY_GC_TIME_MS, QUERY_STALE_TIME_MS } from "@/config/default-constants";

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
      staleTime: QUERY_STALE_TIME_MS,
      gcTime: QUERY_GC_TIME_MS,
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
