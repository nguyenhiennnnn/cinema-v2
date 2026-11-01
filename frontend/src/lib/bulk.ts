import { toast } from "@/components/ui/toast";
import type { BulkActionResponse } from "@/types/bulk";

export function toastBulkResult(
  result: BulkActionResponse | undefined,
  verb: string,
  itemLabel: string,
) {
  if (!result) return;
  const { totalSuccess, totalFailed, results } = result;
  const failedReasons = results
    .filter((r) => !r.success && r.reason)
    .map((r) => `#${r.id}: ${r.reason}`)
    .join("; ");
  toast.add({
    title: `Đã ${verb} ${totalSuccess} ${itemLabel}${totalFailed > 0 ? `, ${totalFailed} thất bại` : ""}.`,
    description: failedReasons || undefined,
    type: totalFailed > 0 ? "warning" : "success",
  });
}
