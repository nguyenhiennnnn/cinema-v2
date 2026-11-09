import type { QueryClient } from "@tanstack/react-query";
import { queryKeys as k } from "./query-keys";

type KeyList = readonly (readonly unknown[])[];

const dashboard = (): KeyList => [k.adminDashboard.all(), k.adminReports.all()];
const movieViews = (): KeyList => [k.adminMovies.all(), k.movies.all()];
const showtimeViews = (): KeyList => [
  k.adminShowtimes.all(),
  k.showtimes.all(),
];
const bookingViews = (): KeyList => [
  k.adminBookings.all(),
  k.bookings.all(),
  k.adminPayments.all(),
];

export const invalidations = {
  movieChanged: (): KeyList => [
    ...movieViews(),
    ...showtimeViews(),
    k.adminReviews.all(),
    ...dashboard(),
  ],
  showtimeChanged: (): KeyList => [
    ...showtimeViews(),
    k.movies.all(),
    ...dashboard(),
  ],
  showtimeCancelled: (): KeyList => [
    ...invalidations.showtimeChanged(),
    ...bookingViews(),
  ],

  bookingChanged: (): KeyList => [
    ...bookingViews(),
    ...showtimeViews(),
    k.users.me(),
    ...dashboard(),
  ],

  comboChanged: (): KeyList => [k.adminCombos.all(), k.combos.all()],

  roomChanged: (): KeyList => [
    k.adminRooms.all(),
    ...showtimeViews(),
    ...dashboard(),
  ],
  seatLayoutChanged: (): KeyList => [k.adminRooms.all(), ...showtimeViews()],

  genreChanged: (): KeyList => [
    k.adminCatalog.genres(),
    k.catalog.genres(),
    ...movieViews(),
  ],
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
  genreCreated: (): KeyList => [k.adminCatalog.genres(), k.catalog.genres()],
  languageCreated: (): KeyList => [
    k.adminCatalog.languages(),
    k.catalog.languages(),
  ],
  ageRatingCreated: (): KeyList => [
    k.adminCatalog.ageRatings(),
    k.catalog.ageRatings(),
  ],
  actorCreated: (): KeyList => [k.adminCatalog.actors()],
  directorCreated: (): KeyList => [k.adminCatalog.directors()],
  positionCreated: (): KeyList => [k.adminCatalog.positions()],
  screenTypeCreated: (): KeyList => [k.adminCatalog.screenTypes()],
  membershipTierChanged: (): KeyList => [
    k.adminCatalog.membershipTiers(),
    k.catalog.membershipTiers(),
    k.adminUsers.all(),
    k.users.me(),
  ],
  positionChanged: (): KeyList => [
    k.adminCatalog.positions(),
    k.adminEmployees.all(),
  ],
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

  userChanged: (): KeyList => [k.adminUsers.all(), k.adminEmployees.all()],
  employeeChanged: (): KeyList => [
    k.adminEmployees.all(),
    k.adminUsers.all(),
    k.adminReports.all(),
  ],
  profileChanged: (): KeyList => [
    k.users.me(),
    k.auth.me(),
    k.adminUsers.all(),
  ],

  reviewChanged: (): KeyList => [
    k.adminReviews.all(),
    ...movieViews(),
    k.users.myReviews(),
    k.adminDashboard.all(),
  ],

  settingsChanged: (): KeyList => [k.adminSettings.get(), k.settings.get()],
} satisfies Record<string, () => KeyList>;

export type InvalidationName = keyof typeof invalidations;

export type AppMutationMeta = {
  skipErrorToast?: boolean;
  skipSuccessToast?: boolean;
  invalidates?: InvalidationName;
};

declare module "@tanstack/react-query" {
  interface Register {
    mutationMeta: AppMutationMeta;
  }
}

function trimTrailingUndefined(key: readonly unknown[]): readonly unknown[] {
  let end = key.length;
  while (end > 0 && key[end - 1] === undefined) end -= 1;
  return key.slice(0, end);
}

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
    [...unique.values()].map((queryKey) =>
      client.invalidateQueries({ queryKey }),
    ),
  );
}
