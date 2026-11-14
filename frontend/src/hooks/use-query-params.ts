import {
  debounce,
  parseAsInteger,
  parseAsString,
  parseAsStringLiteral,
  useQueryStates,
} from "nuqs";
import type { MovieStatus } from "@/types/enum";
import {
  DEFAULT_MOVIES_PAGE_LIMIT,
  DEFAULT_PAGE,
  DEFAULT_PAGE_SIZE,
  SEARCH_DEBOUNCE_MS,
} from "@/config/default-constants";

const searchParser = parseAsString.withDefault("").withOptions({
  limitUrlUpdates: debounce(SEARCH_DEBOUNCE_MS),
  shallow: false,
});

const adminListParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
};

export const useAdminListParams = (
  options?: Parameters<typeof useQueryStates<typeof adminListParsers>>[1],
) => useQueryStates(adminListParsers, options);

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

const adminUserParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
  roleId: parseAsInteger,
};

export const useAdminUserParams = (
  options?: Parameters<typeof useQueryStates<typeof adminUserParsers>>[1],
) => useQueryStates(adminUserParsers, options);

const adminEmployeeParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  positionId: parseAsInteger,
};

export const useAdminEmployeeParams = (
  options?: Parameters<typeof useQueryStates<typeof adminEmployeeParsers>>[1],
) => useQueryStates(adminEmployeeParsers, options);

const adminReviewParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  movieId: parseAsInteger,
  rating: parseAsInteger,
};

export const useAdminReviewParams = (
  options?: Parameters<typeof useQueryStates<typeof adminReviewParsers>>[1],
) => useQueryStates(adminReviewParsers, options);

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

const adminImportExportJobParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  type: parseAsString,
  module: parseAsString,
  status: parseAsString,
};

export const useAdminImportExportJobParams = (
  options?: Parameters<
    typeof useQueryStates<typeof adminImportExportJobParsers>
  >[1],
) => useQueryStates(adminImportExportJobParsers, options);

const adminPersonParsers = {
  page: parseAsInteger.withDefault(DEFAULT_PAGE),
  pageSize: parseAsInteger.withDefault(DEFAULT_PAGE_SIZE),
  search: searchParser,
};

export const useAdminPersonParams = (
  options?: Parameters<typeof useQueryStates<typeof adminPersonParsers>>[1],
) => useQueryStates(adminPersonParsers, options);

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

const scheduleParsers = {
  date: parseAsString.withDefault(""),
  format: parseAsString,
};

export const useScheduleParams = (
  options?: Parameters<typeof useQueryStates<typeof scheduleParsers>>[1],
) => useQueryStates(scheduleParsers, options);
