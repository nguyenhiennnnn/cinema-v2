export const queryKeys = {
  // ── Auth ────────────────────────────────────────────────────────────────────
  auth: {
    me: () => ["auth", "me"] as const,
  },

  // ── Movies ──────────────────────────────────────────────────────────────────
  movies: {
    all: () => ["movies"] as const,
    lists: () => ["movies", "list"] as const,
    list: (params: object) => ["movies", "list", params] as const,
    detail: (movieId: number) => ["movies", "detail", movieId] as const,
    reviews: (movieId: number, params?: object) =>
      ["movies", movieId, "reviews", params] as const,
    canReview: (movieId: number) => ["movies", movieId, "can-review"] as const,
  },

  // ── Showtimes (client) ───────────────────────────────────────────────────────
  showtimes: {
    all: () => ["showtimes"] as const,
    schedule: (date: string) => ["showtimes", "schedule", date] as const,
    seats: (showtimeId: number) => ["showtimes", "seats", showtimeId] as const,
    seatSuggestions: (showtimeId: number, count: number) =>
      ["showtimes", "seat-suggestions", showtimeId, count] as const,
  },

  // ── Bookings (client) ────────────────────────────────────────────────────────
  bookings: {
    all: () => ["bookings"] as const,
    myList: (params?: object) => ["bookings", "my", params] as const,
    detail: (bookingId: number) => ["bookings", "detail", bookingId] as const,
  },

  // ── Combos (client) ──────────────────────────────────────────────────────────
  combos: {
    all: () => ["combos"] as const,
    active: () => ["combos", "active"] as const,
    suggestions: (showtimeId: number, seatCount: number) =>
      ["combos", "suggestions", showtimeId, seatCount] as const,
  },

  // ── Catalog (client) ─────────────────────────────────────────────────────────
  catalog: {
    all: () => ["catalog"] as const,
    genres: () => ["catalog", "genres"] as const,
    languages: () => ["catalog", "languages"] as const,
    ageRatings: () => ["catalog", "age-ratings"] as const,
    membershipTiers: () => ["catalog", "membership-tiers"] as const,
  },

  // ── Settings (client) ────────────────────────────────────────────────────────
  settings: {
    get: () => ["settings"] as const,
  },

  // ── User profile ─────────────────────────────────────────────────────────────
  users: {
    me: () => ["users", "me"] as const,
    myReviews: (params?: object) => ["users", "me", "reviews", params] as const,
  },

  // ── AI ──────────────────────────────────────────────────────────────────────
  ai: {
    chat: () => ["ai", "chat"] as const,
  },

  // ── Admin: Movies ────────────────────────────────────────────────────────────
  adminMovies: {
    all: () => ["admin", "movies"] as const,
    list: (params: object) => ["admin", "movies", "list", params] as const,
  },

  // ── Admin: Showtimes ─────────────────────────────────────────────────────────
  adminShowtimes: {
    all: () => ["admin", "showtimes"] as const,
    list: (params: object) => ["admin", "showtimes", "list", params] as const,
    lists: () => ["admin", "showtimes", "list"] as const,
    calendars: () => ["admin", "showtimes", "calendar"] as const,
    calendar: (params: object) =>
      ["admin", "showtimes", "calendar", params] as const,
  },

  // ── Admin: Bookings ──────────────────────────────────────────────────────────
  adminBookings: {
    all: () => ["admin", "bookings"] as const,
    list: (params: object) => ["admin", "bookings", "list", params] as const,
    detail: (bookingId: number) =>
      ["admin", "bookings", "detail", bookingId] as const,
  },

  // ── Admin: Payments ──────────────────────────────────────────────────────────
  adminPayments: {
    all: () => ["admin", "payments"] as const,
    list: (params: object) => ["admin", "payments", "list", params] as const,
    detail: (paymentId: number) =>
      ["admin", "payments", "detail", paymentId] as const,
  },

  // ── Admin: Rooms ─────────────────────────────────────────────────────────────
  adminRooms: {
    all: () => ["admin", "rooms"] as const,
    list: (params?: object) => ["admin", "rooms", "list", params] as const,
    detail: (roomId: number) => ["admin", "rooms", "detail", roomId] as const,
    seats: (roomId: number) => ["admin", "rooms", "seats", roomId] as const,
  },

  // ── Admin: Combos ────────────────────────────────────────────────────────────
  adminCombos: {
    all: () => ["admin", "combos"] as const,
    list: (params?: object) => ["admin", "combos", "list", params] as const,
  },

  // ── Admin: Reviews ───────────────────────────────────────────────────────────
  adminReviews: {
    all: () => ["admin", "reviews"] as const,
    list: (params: object) => ["admin", "reviews", "list", params] as const,
  },

  // ── Admin: Users ─────────────────────────────────────────────────────────────
  adminUsers: {
    all: () => ["admin", "users"] as const,
    list: (params: object) => ["admin", "users", "list", params] as const,
    admins: (params?: object) => ["admin", "users", "admins", params] as const,
    customers: (params?: object) => ["admin", "users", "customers", params] as const,
  },

  // ── Admin: Employees ─────────────────────────────────────────────────────────
  adminEmployees: {
    all: () => ["admin", "employees"] as const,
    list: (params?: object) => ["admin", "employees", "list", params] as const,
  },

  // ── Admin: Settings ──────────────────────────────────────────────────────────
  adminSettings: {
    get: () => ["admin", "settings"] as const,
  },

  // ── Admin: Dashboard ─────────────────────────────────────────────────────────
  adminDashboard: {
    all: () => ["admin", "dashboard"] as const,
    summary: () => ["admin", "dashboard", "summary"] as const,
    insights: (params: object) =>
      ["admin", "dashboard", "insights", params] as const,
  },

  // ── Admin: Reports ───────────────────────────────────────────────────────────
  adminReports: {
    all: () => ["admin", "reports"] as const,
    revenue: (params: object) =>
      ["admin", "reports", "revenue", params] as const,
    topMovies: (params: object) =>
      ["admin", "reports", "top-movies", params] as const,
    employeePerformance: (params: object) =>
      ["admin", "reports", "employee-performance", params] as const,
  },

  // ── Admin: Catalog ───────────────────────────────────────────────────────────
  adminCatalog: {
    all: () => ["admin", "catalog"] as const,
    genres: (params?: object) =>
      ["admin", "catalog", "genres", params] as const,
    languages: (params?: object) =>
      ["admin", "catalog", "languages", params] as const,
    ageRatings: (params?: object) =>
      ["admin", "catalog", "age-ratings", params] as const,
    seatTypes: (params?: object) =>
      ["admin", "catalog", "seat-types", params] as const,
    screenTypes: (params?: object) =>
      ["admin", "catalog", "screen-types", params] as const,
    membershipTiers: (params?: object) =>
      ["admin", "catalog", "membership-tiers", params] as const,
    positions: (params?: object) =>
      ["admin", "catalog", "positions", params] as const,
    actors: (params?: object) =>
      ["admin", "catalog", "actors", params] as const,
    directors: (params?: object) =>
      ["admin", "catalog", "directors", params] as const,
  },
} as const;
