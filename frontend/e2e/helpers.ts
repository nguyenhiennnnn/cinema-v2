import type { Page } from "@playwright/test";
import { execFile } from "child_process";
import { promisify } from "util";

const execFileAsync = promisify(execFile);

export const CUSTOMER_CREDENTIALS = {
  email: process.env.E2E_CUSTOMER_EMAIL ?? "customer@cinema.local",
  password: process.env.E2E_CUSTOMER_PASSWORD ?? "Customer@123",
};

export const ADMIN_CREDENTIALS = {
  email: process.env.E2E_ADMIN_EMAIL ?? "admin@cinema.local",
  password: process.env.E2E_ADMIN_PASSWORD ?? "Admin@123",
};

const API_BASE_URL =
  process.env.VITE_API_BASE_URL ?? "http://localhost:8080/api/v1";

const PSQL_IMAGE = process.env.E2E_PSQL_IMAGE ?? "postgres:17-alpine";

function getDatabaseUrl() {
  return process.env.E2E_DATABASE_URL ?? process.env.DATABASE_URL;
}

async function runSql(sql: string): Promise<boolean> {
  const dbUrl = getDatabaseUrl();
  if (!dbUrl) {
    console.warn("Could not run SQL: DATABASE_URL is not set");
    return false;
  }
  try {
    await execFileAsync(
      "docker",
      [
        "run",
        "--rm",
        "-e",
        "DATABASE_URL",
        "-e",
        "E2E_SQL",
        PSQL_IMAGE,
        "sh",
        "-c",
        'psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -c "$E2E_SQL"',
      ],
      {
        timeout: 120_000,
        env: { ...process.env, DATABASE_URL: dbUrl, E2E_SQL: sql },
      },
    );
    return true;
  } catch (err) {
    console.warn(
      "Could not run SQL via psql:",
      err instanceof Error ? err.message : err,
    );
    return false;
  }
}

let isApiHealthy = false;

export async function waitForApiReady(page: Page, timeoutMs = 60_000): Promise<void> {
  if (isApiHealthy) return;
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    try {
      const res = await page.request.get(`${API_BASE_URL}/health`, { timeout: 3_000 });
      if (res.ok()) {
        isApiHealthy = true;
        return;
      }
    } catch {
      // API might be compiling or restarting; retry until timeout
    }
    await new Promise((r) => setTimeout(r, 2_000));
  }
  throw new Error(`API at ${API_BASE_URL}/health was not ready within ${timeoutMs}ms`);
}

let customerApiToken: string | null = null;
let adminApiToken: string | null = null;

async function loginForToken(
  page: Page,
  path: string,
  credentials: { email: string; password: string },
): Promise<string | null> {
  await waitForApiReady(page);
  const res = await page.request.post(`${API_BASE_URL}${path}`, {
    data: { email: credentials.email, password: credentials.password },
  });
  if (!res.ok()) return null;
  const body = await res.json();
  return body.data?.accessToken ?? null;
}

export async function getCustomerApiToken(page: Page): Promise<string | null> {
  customerApiToken ??= await loginForToken(
    page,
    "/auth/login",
    CUSTOMER_CREDENTIALS,
  );
  return customerApiToken;
}

async function getAdminApiToken(page: Page): Promise<string | null> {
  adminApiToken ??= await loginForToken(
    page,
    "/auth/admin-login",
    ADMIN_CREDENTIALS,
  );
  return adminApiToken;
}

export async function ensureNoPendingBookings(page: Page) {
  try {
    const apiContext = page.request;
    const token = await getCustomerApiToken(page);
    if (!token) return;

    const bookingsRes = await apiContext.get(
      `${API_BASE_URL}/bookings?status=Pending`,
      {
        headers: { Authorization: `Bearer ${token}` },
      },
    );
    if (bookingsRes.ok()) {
      const listBody = await bookingsRes.json();
      const items = listBody.data?.items ?? [];
      for (const item of items) {
        await apiContext.post(
          `${API_BASE_URL}/bookings/${item.bookingId}/cancel`,
          {
            headers: { Authorization: `Bearer ${token}` },
          },
        );
      }
    }
  } catch {
    // Non-fatal cleanup
  }
}

export async function loginAsCustomer(page: Page) {
  await waitForApiReady(page);
  await ensureNoPendingBookings(page);

  const MAX_ATTEMPTS = 3;
  for (let attempt = 1; attempt <= MAX_ATTEMPTS; attempt++) {
    await page.goto("/auth/login", { waitUntil: "domcontentloaded" });
    await page.getByLabel("Email").fill(CUSTOMER_CREDENTIALS.email!);
    await page
      .getByLabel("Mật khẩu", { exact: true })
      .fill(CUSTOMER_CREDENTIALS.password!);
    await page.getByRole("button", { name: "Đăng nhập" }).click();

    const redirected = await page
      .waitForURL((url) => !url.pathname.startsWith("/auth/login"), {
        waitUntil: "commit",
        timeout: 15_000,
      })
      .then(() => true)
      .catch(() => false);

    if (redirected) return;

    if (attempt < MAX_ATTEMPTS) {
      // Transient DB failure (e.g. Neon cold-start) – wait briefly then retry
      await page.waitForTimeout(2_000);
    }
  }

  // Final hard assertion so the test fails with a clear message
  await page.waitForURL((url) => !url.pathname.startsWith("/auth/login"), {
    waitUntil: "commit",
    timeout: 15_000,
  });
}

export async function loginAsAdmin(page: Page) {
  await waitForApiReady(page);
  const MAX_ATTEMPTS = 3;
  for (let attempt = 1; attempt <= MAX_ATTEMPTS; attempt++) {
    await page.goto("/admin/login", { waitUntil: "domcontentloaded" });
    await page.getByLabel("Email quản trị").fill(ADMIN_CREDENTIALS.email!);
    await page
      .getByLabel("Mật khẩu", { exact: true })
      .fill(ADMIN_CREDENTIALS.password!);
    await page.getByRole("button", { name: "Đăng nhập quản trị" }).click();

    const redirected = await page
      .waitForURL((url) => !url.pathname.startsWith("/admin/login"), {
        waitUntil: "commit",
        timeout: 15_000,
      })
      .then(() => true)
      .catch(() => false);

    if (redirected) return;

    if (attempt < MAX_ATTEMPTS) {
      // Transient DB failure (e.g. Neon cold-start) – wait briefly then retry
      await page.waitForTimeout(2_000);
    }
  }

  // Final hard assertion so the test fails with a clear message
  await page.waitForURL((url) => !url.pathname.startsWith("/admin/login"), {
    waitUntil: "commit",
    timeout: 15_000,
  });
}

export interface TestTicketInfo {
  ticketId: number;
  qrCode: string;
  movieTitle: string;
  seatRow: string;
  seatNumber: number;
}

export async function getLatestPendingBookingId(
  page: Page,
  token: string,
): Promise<number | null> {
  const apiContext = page.request;
  const bookingsRes = await apiContext.get(
    `${API_BASE_URL}/bookings?status=Pending`,
    { headers: { Authorization: `Bearer ${token}` } },
  );
  if (!bookingsRes.ok()) return null;
  const body = await bookingsRes.json();
  const items = body.data?.items ?? [];
  if (items.length === 0) return null;
  return Math.max(...items.map((i: { bookingId: number }) => i.bookingId));
}

export async function expireBookingHoldInDb(
  bookingId: number,
): Promise<boolean> {
  return runSql(
    `UPDATE "Bookings" SET "ExpiresAt" = (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 minute' WHERE "BookingId" = ${bookingId};`,
  );
}

export async function cancelBookingViaApi(
  page: Page,
  token: string,
  bookingId: number,
): Promise<boolean> {
  const res = await page.request.post(
    `${API_BASE_URL}/bookings/${bookingId}/cancel`,
    { headers: { Authorization: `Bearer ${token}` } },
  );
  return res.ok();
}

export async function holdFirstAvailableSeat(
  page: Page,
  token: string,
  showtimeId = 1,
): Promise<{ bookingId: number; bookingCode: string } | null> {
  const apiContext = page.request;
  const seatsRes = await apiContext.get(
    `${API_BASE_URL}/showtimes/${showtimeId}/seats`,
    { headers: { Authorization: `Bearer ${token}` } },
  );
  if (!seatsRes.ok()) return null;
  const seatsBody = await seatsRes.json();
  const availableSeat = (seatsBody.data?.seats ?? []).find(
    (s: { status: string }) => s.status === "Available",
  );
  if (!availableSeat) return null;

  const holdRes = await apiContext.post(`${API_BASE_URL}/bookings/hold`, {
    headers: { Authorization: `Bearer ${token}` },
    data: {
      showtimeId,
      seatIds: [availableSeat.showtimeSeatId],
      customerName: "E2E Test User",
      customerPhone: "0912345678",
    },
  });
  if (!holdRes.ok()) return null;
  const holdBody = await holdRes.json();
  return {
    bookingId: holdBody.data.bookingId,
    bookingCode: holdBody.data.bookingCode,
  };
}

export async function createEligibleTicketForCheckin(
  page: Page,
): Promise<TestTicketInfo | null> {
  const apiContext = page.request;

  try {
    await runSql(
      `UPDATE "Bookings" SET "Status" = 'Cancelled', "ExpiresAt" = (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 minute' WHERE "ShowtimeId" = 1 AND "Status" = 'Pending';
       UPDATE "ShowtimeSeats" SET "Status" = 'Available' WHERE "ShowtimeId" = 1 AND "Status" = 'Held';
       UPDATE "Showtimes" SET "StartTime" = (NOW() AT TIME ZONE 'UTC') + INTERVAL '15 minutes', "EndTime" = (NOW() AT TIME ZONE 'UTC') + INTERVAL '135 minutes' WHERE "ShowtimeId" = 1;`,
    );

    // 1. Admin login to obtain admin token
    const adminToken = await getAdminApiToken(page);
    if (!adminToken) return null;

    // 2. Customer login to obtain customer token
    const custToken = await getCustomerApiToken(page);
    if (!custToken) return null;

    // Cancel pending bookings first
    await ensureNoPendingBookings(page);

    // 3. Find available seat in showtime 1
    const seatsRes = await apiContext.get(`${API_BASE_URL}/showtimes/1/seats`, {
      headers: { Authorization: `Bearer ${custToken}` },
    });
    if (!seatsRes.ok()) { console.error("createEligibleTicket: seatsRes failed", await seatsRes.text()); return null; }
    const seatsBody = await seatsRes.json();
    const movieTitle = seatsBody.data?.movieTitle ?? "Phim";
    const availableSeat = (seatsBody.data?.seats ?? []).find(
      (s: { status: string }) => s.status === "Available",
    );
    if (!availableSeat) { console.error("createEligibleTicket: no availableSeat"); return null; }

    // 4. Hold seat as counter booking
    const holdRes = await apiContext.post(`${API_BASE_URL}/admin/bookings/counter-hold`, {
      headers: { Authorization: `Bearer ${adminToken}` },
      data: {
        showtimeId: 1,
        seatIds: [availableSeat.showtimeSeatId],
        customerName: "Khách lẻ E2E",
        customerPhone: "0912345678",
      },
    });
    if (!holdRes.ok()) { console.error("createEligibleTicket: holdRes failed", await holdRes.text()); return null; }
    const holdBody = await holdRes.json();
    const bookingId = holdBody.data.bookingId;
    const bookingCode = holdBody.data.bookingCode;

    // 5. Complete counter checkout as admin
    const checkoutRes = await apiContext.post(
      `${API_BASE_URL}/admin/bookings/${bookingId}/counter-checkout`,
      {
        headers: { Authorization: `Bearer ${adminToken}` },
        data: { amountReceived: 500000 },
      },
    );
    if (!checkoutRes.ok()) { console.error("createEligibleTicket: checkoutRes failed", await checkoutRes.text()); return null; }

    // 6. QR code is ${bookingCode}-${seatRow}${seatNumber}
    const qrCode = `${bookingCode}-${availableSeat.seatRow}${availableSeat.seatNumber}`;

    return {
      ticketId: bookingId,
      qrCode,
      movieTitle,
      seatRow: availableSeat.seatRow,
      seatNumber: availableSeat.seatNumber,
    };
  } catch (err) {
    console.error("createEligibleTicketForCheckin failed:", err);
    return null;
  }
}

export async function cleanFutureTestShowtimes() {
  await runSql(
    'DELETE FROM "ShowtimeSeats" WHERE "ShowtimeId" IN (SELECT "ShowtimeId" FROM "Showtimes" WHERE "StartTime" >= \'2026-12-01\'); DELETE FROM "Showtimes" WHERE "StartTime" >= \'2026-12-01\';',
  );
}

