import type { QueryClient } from "@tanstack/react-query";
import { queryKeys as k } from "./query-keys";

/**
 * Bản đồ tập trung: "mutation thay đổi dữ liệu gì → query nào phải làm mới".
 *
 * Cách dùng: khai báo ở hook mutation
 *   useMutation({ meta: { invalidates: "movieChanged" }, mutationFn })
 * `MutationCache.onSuccess` (query-client.ts) sẽ gọi `invalidateRelated` và đợi refetch
 * các query đang hiển thị xong mới resolve mutation, nên UI không nhấp nháy dữ liệu cũ.
 *
 * Thêm quan hệ mới: bổ sung 1 mục vào `invalidations` bên dưới (đừng gọi invalidateQueries trong hook).
 */

type KeyList = readonly (readonly unknown[])[];

const dashboard = (): KeyList => [k.adminDashboard.all(), k.adminReports.all()];
const movieViews = (): KeyList => [k.adminMovies.all(), k.movies.all()];
const showtimeViews = (): KeyList => [k.adminShowtimes.all(), k.showtimes.all()];
const bookingViews = (): KeyList => [
  k.adminBookings.all(),
  k.bookings.all(),
  k.adminPayments.all(),
];

export const invalidations = {
  // ── Phim / suất chiếu ──────────────────────────────────────────────────────
  movieChanged: (): KeyList => [
    ...movieViews(),
    ...showtimeViews(),
    k.adminReviews.all(),
    ...dashboard(),
  ],
  showtimeChanged: (): KeyList => [...showtimeViews(), k.movies.all(), ...dashboard()],
  /** Hủy/xóa suất chiếu: vé bị hủy và giao dịch được hoàn tiền. */
  showtimeCancelled: (): KeyList => [
    ...invalidations.showtimeChanged(),
    ...bookingViews(),
  ],

  // ── Đặt vé / thanh toán ────────────────────────────────────────────────────
  bookingChanged: (): KeyList => [
    ...bookingViews(),
    ...showtimeViews(), // sơ đồ ghế của suất chiếu
    k.users.me(), // điểm tích lũy
    ...dashboard(),
  ],

  // ── Combo ──────────────────────────────────────────────────────────────────
  comboChanged: (): KeyList => [k.adminCombos.all(), k.combos.all()],

  // ── Phòng / ghế ────────────────────────────────────────────────────────────
  roomChanged: (): KeyList => [k.adminRooms.all(), ...showtimeViews(), ...dashboard()],
  seatLayoutChanged: (): KeyList => [k.adminRooms.all(), ...showtimeViews()],

  // ── Danh mục (gồm các danh sách chọn trong form) ───────────────────────────
  genreChanged: (): KeyList => [k.adminCatalog.genres(), k.catalog.genres(), ...movieViews()],
  languageChanged: (): KeyList => [
    k.adminCatalog.languages(),
    k.catalog.languages(),
    ...movieViews(),
  ],
  ageRatingChanged: (): KeyList => [
    k.adminCatalog.ageRatings(),
    k.catalog.ageRatings(),
    ...movieViews(),
  ],
  actorChanged: (): KeyList => [k.adminCatalog.actors(), ...movieViews()],
  directorChanged: (): KeyList => [k.adminCatalog.directors(), ...movieViews()],
  membershipTierChanged: (): KeyList => [
    k.adminCatalog.membershipTiers(),
    k.catalog.membershipTiers(),
    k.adminUsers.all(),
    k.users.me(),
  ],
  positionChanged: (): KeyList => [k.adminCatalog.positions(), k.adminEmployees.all()],
  screenTypeChanged: (): KeyList => [
    k.adminCatalog.screenTypes(),
    k.adminRooms.all(),
    ...showtimeViews(),
  ],
  seatTypeChanged: (): KeyList => [
    k.adminCatalog.seatTypes(),
    k.adminRooms.all(),
    ...showtimeViews(),
  ],

  // ── Người dùng / nhân viên ─────────────────────────────────────────────────
  /** Khóa/mở khóa, đổi vai trò. */
  userChanged: (): KeyList => [k.adminUsers.all(), k.adminEmployees.all()],
  employeeChanged: (): KeyList => [
    k.adminEmployees.all(),
    k.adminUsers.all(),
    k.adminReports.all(),
  ],
  profileChanged: (): KeyList => [k.users.me(), k.auth.me(), k.adminUsers.all()],

  // ── Đánh giá (admin và client) ─────────────────────────────────────────────
  reviewChanged: (): KeyList => [
    k.adminReviews.all(),
    ...movieViews(),
    k.users.myReviews(),
    k.adminDashboard.all(),
  ],

  // ── Cấu hình ───────────────────────────────────────────────────────────────
  settingsChanged: (): KeyList => [k.adminSettings.get(), k.settings.get()],
} satisfies Record<string, () => KeyList>;

export type InvalidationName = keyof typeof invalidations;

/** Kiểu cho `mutation.meta` (toàn dự án). Mutation không khai báo `invalidates` thì không làm mới gì. */
export type AppMutationMeta = {
  /** Tắt toast lỗi toàn cục. */
  skipErrorToast?: boolean;
  /** Tắt toast thành công toàn cục (mutation tự toast tổng hợp). */
  skipSuccessToast?: boolean;
  /** Tên quan hệ trong `invalidations`. */
  invalidates?: InvalidationName;
};

declare module "@tanstack/react-query" {
  interface Register {
    mutationMeta: AppMutationMeta;
  }
}

/**
 * Key dựng với tham số tùy chọn (vd `genres()`) kết thúc bằng `undefined`;
 * TanStack không coi nó là prefix của key có params, nên phải cắt đi trước khi invalidate.
 */
function trimTrailingUndefined(key: readonly unknown[]): readonly unknown[] {
  let end = key.length;
  while (end > 0 && key[end - 1] === undefined) end -= 1;
  return key.slice(0, end);
}

/** Làm mới mọi query liên quan; resolve khi các query đang hiển thị đã refetch xong. */
export async function invalidateRelated(
  client: QueryClient,
  name: InvalidationName | undefined,
): Promise<void> {
  if (!name) return;
  const unique = new Map<string, readonly unknown[]>();
  for (const key of invalidations[name]()) {
    const trimmed = trimTrailingUndefined(key);
    unique.set(JSON.stringify(trimmed), trimmed);
  }
  await Promise.all(
    [...unique.values()].map((queryKey) => client.invalidateQueries({ queryKey })),
  );
}
