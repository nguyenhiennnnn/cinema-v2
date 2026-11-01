import { QueryCache, QueryClient, MutationCache } from "@tanstack/react-query";
import { extractApiError, logDevError, showApiErrorToast } from "@/lib/error";
import { getApiSuccessMessage } from "@/lib/api-response";
import { invalidateRelated } from "@/api/query-invalidation";
import { toast } from "@/components/ui/toast";
import { QUERY_GC_TIME_MS, QUERY_STALE_TIME_MS } from "@/config/default-constants";

function handleGlobalError(context: "Query" | "Mutation") {
  return (error: unknown, query: { meta?: Record<string, unknown> }) => {
    logDevError(error, context);
    if (query?.meta?.skipErrorToast) return;
    showApiErrorToast(error);
  };
}

/**
 * Hiện toast thành công từ `message` của server.
 * Tắt cho từng mutation bằng `meta: { skipSuccessToast: true }`
 * (vd: bulk tự toast tổng hợp "x thành công, y thất bại").
 */
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
    // Trả promise: mutation chỉ resolve sau khi dữ liệu liên quan đã refetch (xem query-invalidation.ts)
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
