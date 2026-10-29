import {
  debounce,
  parseAsInteger,
  parseAsString,
  parseAsStringLiteral,
  useQueryStates,
} from "nuqs";
import type { MovieStatus } from "@/types/enum";
import {
  DEFAULT_BOOKINGS_PAGE_LIMIT,
  DEFAULT_MOVIES_PAGE_LIMIT,
  DEFAULT_PAGE,
  DEFAULT_PAGE_SIZE,
  SEARCH_DEBOUNCE_MS,
} from "@/config/default-constants";

const searchParser = parseAsString.withDefault("").withOptions({
  limitUrlUpdates: debounce(SEARCH_DEBOUNCE_MS),
  shallow: false,
});

// ── Common admin offset-paginated list ────────────────────────────────────────

const adminListParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
};

export const useAdminListParams = (
  options?: Parameters<typeof useQueryStates<typeof adminListParsers>>[1],
) => useQueryStates(adminListParsers, options);

// ── Admin movies ──────────────────────────────────────────────────────────────

const movieStatuses = [
  "ComingSoon",
  "NowShowing",
  "Ended",
] as const satisfies MovieStatus[];

const adminMovieParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
  status: parseAsStringLiteral(movieStatuses),
  directorId: parseAsInteger,
  languageId: parseAsInteger,
  ageRatingId: parseAsInteger,
  genreId: parseAsInteger,
};

export const useAdminMovieParams = (
  options?: Parameters<typeof useQueryStates<typeof adminMovieParsers>>[1],
) => useQueryStates(adminMovieParsers, options);

// ── Admin showtimes ───────────────────────────────────────────────────────────

const adminShowtimeParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  movieId: parseAsInteger,
  roomId: parseAsInteger,
  date: parseAsString,
  status: parseAsString,
};

export const useAdminShowtimeParams = (
  options?: Parameters<typeof useQueryStates<typeof adminShowtimeParsers>>[1],
) => useQueryStates(adminShowtimeParsers, options);

// ── Admin showtimes calendar ─────────────────────────────────

const adminShowtimeCalendarParsers = {
  from: parseAsString,
  to: parseAsString,
  roomId: parseAsInteger,
  movieId: parseAsInteger,
  status: parseAsString,
};

export const useAdminShowtimeCalendarParams = (
  options?: Parameters<
    typeof useQueryStates<typeof adminShowtimeCalendarParsers>
  >[1],
) => useQueryStates(adminShowtimeCalendarParsers, options);

// ── Admin bookings ────────────────────────────────────────────────────────────

const adminBookingParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
  status: parseAsString,
  channel: parseAsString,
  fromDate: parseAsString,
  toDate: parseAsString,
};

export const useAdminBookingParams = (
  options?: Parameters<typeof useQueryStates<typeof adminBookingParsers>>[1],
) => useQueryStates(adminBookingParsers, options);

// ── Admin users ───────────────────────────────────────────────────────────────

const adminUserParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
  roleId: parseAsInteger,
};

export const useAdminUserParams = (
  options?: Parameters<typeof useQueryStates<typeof adminUserParsers>>[1],
) => useQueryStates(adminUserParsers, options);

// ── Admin employees ───────────────────────────────────────────────────────────

const adminEmployeeParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  positionId: parseAsInteger,
};

export const useAdminEmployeeParams = (
  options?: Parameters<typeof useQueryStates<typeof adminEmployeeParsers>>[1],
) => useQueryStates(adminEmployeeParsers, options);

// ── Admin reviews ─────────────────────────────────────────────────────────────

const adminReviewParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  movieId: parseAsInteger,
  rating: parseAsInteger,
};

export const useAdminReviewParams = (
  options?: Parameters<typeof useQueryStates<typeof adminReviewParsers>>[1],
) => useQueryStates(adminReviewParsers, options);

// ── Admin payments ────────────────────────────────────────────────────────────

const adminPaymentParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
  status: parseAsString,
  paymentMethod: parseAsString,
  fromDate: parseAsString,
  toDate: parseAsString,
};

export const useAdminPaymentParams = (
  options?: Parameters<typeof useQueryStates<typeof adminPaymentParsers>>[1],
) => useQueryStates(adminPaymentParsers, options);

// ── Admin catalog (actors / directors search) ─────────────────────────────────

const adminPersonParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
};

export const useAdminPersonParams = (
  options?: Parameters<typeof useQueryStates<typeof adminPersonParsers>>[1],
) => useQueryStates(adminPersonParsers, options);

// ── Admin reports ─────────────────────────────────────────────────────────────

const adminReportParsers = {
  fromDate: parseAsString.withDefault(""),
  toDate: parseAsString.withDefault(""),
  groupBy: parseAsString.withDefault("day"),
};

export const useAdminReportParams = (
  options?: Parameters<typeof useQueryStates<typeof adminReportParsers>>[1],
) => useQueryStates(adminReportParsers, options);

// ── Client movies ─────────────────────────────────────────────────────────────

const clientMovieParsers = {
  cursor: parseAsString,
  limit: parseAsInteger.withDefault(DEFAULT_MOVIES_PAGE_LIMIT),
  status: parseAsString,
  genreId: parseAsInteger,
  languageId: parseAsInteger,
  ageRatingId: parseAsInteger,
  search: searchParser,
};

export const useClientMovieParams = (
  options?: Parameters<typeof useQueryStates<typeof clientMovieParsers>>[1],
) => useQueryStates(clientMovieParsers, options);

// ── Client bookings ───────────────────────────────────────────────────────────

const clientBookingParsers = {
  cursor: parseAsString,
  limit: parseAsInteger.withDefault(DEFAULT_BOOKINGS_PAGE_LIMIT),
  status: parseAsString,
};

export const useClientBookingParams = (
  options?: Parameters<typeof useQueryStates<typeof clientBookingParsers>>[1],
) => useQueryStates(clientBookingParsers, options);

// ── Client schedule ───────────────────────────────────────────────────────────

const scheduleParsers = {
  date: parseAsString.withDefault(""),
};

export const useScheduleParams = (
  options?: Parameters<typeof useQueryStates<typeof scheduleParsers>>[1],
) => useQueryStates(scheduleParsers, options);

// ── Re-export commonly used nuqs parsers for ad-hoc use ──────────────────────

export {
  debounce,
  parseAsInteger,
  parseAsString,
  parseAsStringLiteral,
  useQueryStates,
};
