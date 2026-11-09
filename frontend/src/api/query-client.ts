import { QueryCache, QueryClient, MutationCache } from "@tanstack/react-query";
import { extractApiError, handleApiError } from "@/lib/error";
import { getApiSuccessMessage } from "@/lib/api-response";
import { invalidateRelated } from "@/api/query-invalidation";
import { toast } from "@/components/ui/toast";
import {
  MUTATION_MAX_RETRIES,
  NO_RETRY_STATUS_MAX,
  NO_RETRY_STATUS_MIN,
  QUERY_GC_TIME_MS,
  QUERY_MAX_RETRIES,
  QUERY_STALE_TIME_MS,
} from "@/config/default-constants";

function handleGlobalError(context: "Query" | "Mutation") {
  return (error: unknown, query: { meta?: Record<string, unknown> }) => {
    const title = query?.meta?.errorTitle;
    handleApiError(error, {
      context,
      silent: Boolean(query?.meta?.skipErrorToast),
      title: typeof title === "string" ? title : undefined,
    });
  };
}

function handleGlobalSuccess(
  data: unknown,
  mutation: { meta?: Record<string, unknown> },
) {
  if (mutation.meta?.skipSuccessToast) return;
  const message = getApiSuccessMessage(data);
  if (!message) return;
  toast.add({ title: message, type: "success" });
}

export const queryClient: QueryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error, query) => handleGlobalError("Query")(error, query),
  }),
  mutationCache: new MutationCache({
    onSuccess: (data, _variables, _context, mutation) => {
      handleGlobalSuccess(data, mutation);
      return invalidateRelated(queryClient, mutation.meta?.invalidates);
    },
    onError: (error, _variables, _context, mutation) =>
      handleGlobalError("Mutation")(error, mutation),
  }),
  defaultOptions: {
    queries: {
      staleTime: QUERY_STALE_TIME_MS,
      gcTime: QUERY_GC_TIME_MS,
      retry: (failureCount, error) => {
        const { statusCode } = extractApiError(error);
        if (
          statusCode &&
          statusCode >= NO_RETRY_STATUS_MIN &&
          statusCode < NO_RETRY_STATUS_MAX
        ) {
          return false;
        }
        return failureCount < QUERY_MAX_RETRIES;
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: MUTATION_MAX_RETRIES,
    },
  },
});
