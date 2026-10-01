import type { Page } from "@playwright/test";
import { exec } from "child_process";
import { promisify } from "util";

const execAsync = promisify(exec);

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

const SQL_SA_PASSWORD = process.env.SQL_SA_PASSWORD ?? "Hien@12345";
const SQL_CONTAINER_NAME = process.env.SQL_CONTAINER_NAME ?? "cinema-sqlserver";
const SQL_DB_NAME = process.env.SQL_DB_NAME ?? "CinemaDb";

export async function ensureNoPendingBookings(page: Page) {
  try {
    const apiContext = page.request;
    const loginRes = await apiContext.post(`${API_BASE_URL}/auth/login`, {
      data: {
        email: CUSTOMER_CREDENTIALS.email,
        password: CUSTOMER_CREDENTIALS.password,
      },
    });

    if (loginRes.ok()) {
      const body = await loginRes.json();
      const token = body.data?.accessToken;
      if (token) {
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
      }
    }
  } catch {
    // Non-fatal cleanup
  }
}

export async function loginAsCustomer(page: Page) {
  await ensureNoPendingBookings(page);

  await page.goto("/auth/login", { waitUntil: "domcontentloaded" });
  await page.getByLabel("Email").fill(CUSTOMER_CREDENTIALS.email!);
  await page
    .getByLabel("Mật khẩu", { exact: true })
    .fill(CUSTOMER_CREDENTIALS.password!);
  await page.getByRole("button", { name: "Đăng nhập" }).click();
  await page.waitForURL((url) => !url.pathname.startsWith("/auth/login"), {
    waitUntil: "commit",
    timeout: 15_000,
  });
}

export async function loginAsAdmin(page: Page) {
  await page.goto("/admin/login", { waitUntil: "domcontentloaded" });
  await page.getByLabel("Email quản trị").fill(ADMIN_CREDENTIALS.email!);
  await page
    .getByLabel("Mật khẩu", { exact: true })
    .fill(ADMIN_CREDENTIALS.password!);
  await page.getByRole("button", { name: "Đăng nhập quản trị" }).click();
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

export async function getCustomerApiToken(page: Page): Promise<string | null> {
  const apiContext = page.request;
  const loginRes = await apiContext.post(`${API_BASE_URL}/auth/login`, {
    data: {
      email: CUSTOMER_CREDENTIALS.email,
      password: CUSTOMER_CREDENTIALS.password,
    },
  });
  if (!loginRes.ok()) return null;
  const body = await loginRes.json();
  return body.data?.accessToken ?? null;
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
  try {
    await execAsync(
      `docker exec ${SQL_CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${SQL_SA_PASSWORD}" -d ${SQL_DB_NAME} -C -Q "UPDATE Bookings SET ExpiresAt = DATEADD(minute, -1, GETUTCDATE()) WHERE BookingId = ${bookingId};"`,
      { timeout: 15000 },
    );
    return true;
  } catch (err) {
    console.warn("Could not expire booking hold via sqlcmd:", err);
    return false;
  }
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
    if (SQL_SA_PASSWORD) {
      try {
        const sql =
          "UPDATE Showtimes SET StartTime = DATEADD(minute, 15, GETUTCDATE()), " +
          "EndTime = DATEADD(minute, 135, GETUTCDATE()) WHERE ShowtimeId = 1;";
        await execAsync(
          `docker exec ${SQL_CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd ` +
            `-S localhost -U sa -P "${SQL_SA_PASSWORD}" -d ${SQL_DB_NAME} -C -Q "${sql}"`,
          { timeout: 15000 },
        );
      } catch (err) {
        console.warn("Could not update showtime start time via sqlcmd:", err);
      }
    }

    // 1. Admin login to obtain admin token
    const adminLoginRes = await apiContext.post(
      `${API_BASE_URL}/auth/admin-login`,
      {
        data: {
          email: ADMIN_CREDENTIALS.email,
          password: ADMIN_CREDENTIALS.password,
        },
      },
    );
    if (!adminLoginRes.ok()) return null;
    const adminToken = (await adminLoginRes.json()).data.accessToken;

    // 2. Customer login to obtain customer token
    const custLoginRes = await apiContext.post(`${API_BASE_URL}/auth/login`, {
      data: {
        email: CUSTOMER_CREDENTIALS.email,
        password: CUSTOMER_CREDENTIALS.password,
      },
    });
    if (!custLoginRes.ok()) return null;
    const custToken = (await custLoginRes.json()).data.accessToken;

    // Cancel pending bookings first
    await ensureNoPendingBookings(page);

    // 3. Find available seat in showtime 1
    const seatsRes = await apiContext.get(`${API_BASE_URL}/showtimes/1/seats`, {
      headers: { Authorization: `Bearer ${custToken}` },
    });
    if (!seatsRes.ok()) return null;
    const seatsBody = await seatsRes.json();
    const movieTitle = seatsBody.data?.movieTitle ?? "Phim";
    const availableSeat = (seatsBody.data?.seats ?? []).find(
      (s: { status: string }) => s.status === "Available",
    );
    if (!availableSeat) return null;

    // 4. Hold seat as customer
    const holdRes = await apiContext.post(`${API_BASE_URL}/bookings/hold`, {
      headers: { Authorization: `Bearer ${custToken}` },
      data: {
        showtimeId: 1,
        seatIds: [availableSeat.showtimeSeatId],
        customerName: "E2E Test User",
        customerPhone: "0912345678",
      },
    });
    if (!holdRes.ok()) return null;
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
    if (!checkoutRes.ok()) return null;

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
