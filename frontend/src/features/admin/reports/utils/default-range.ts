import { subDays } from "date-fns";
import { getBusinessTodayDate } from "@/lib/business-time";
import { toDateInputValue } from "@/lib/formatters";

export function getDefaultReportRange(days = 30) {
  const to = getBusinessTodayDate();
  const from = subDays(to, days - 1);
  return { fromDate: toDateInputValue(from) ?? "", toDate: toDateInputValue(to) ?? "" };
}
