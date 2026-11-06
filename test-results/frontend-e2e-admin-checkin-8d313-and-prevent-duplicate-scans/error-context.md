# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: frontend\e2e\admin-checkin-flow.spec.ts >> admin ticket check-in flow: reject invalid codes, successfully check-in valid ticket, and prevent duplicate scans
- Location: frontend\e2e\admin-checkin-flow.spec.ts:6:1

# Error details

```
Error: page.goto: Protocol error (Page.navigate): Cannot navigate to invalid URL
Call log:
  - navigating to "/admin/login", waiting until "domcontentloaded"

```

# Test source

```ts
  84  |   page: Page,
  85  |   path: string,
  86  |   credentials: { email: string; password: string },
  87  | ): Promise<string | null> {
  88  |   await waitForApiReady(page);
  89  |   const res = await page.request.post(`${API_BASE_URL}${path}`, {
  90  |     data: { email: credentials.email, password: credentials.password },
  91  |   });
  92  |   if (!res.ok()) return null;
  93  |   const body = await res.json();
  94  |   return body.data?.accessToken ?? null;
  95  | }
  96  | 
  97  | export async function getCustomerApiToken(page: Page): Promise<string | null> {
  98  |   customerApiToken ??= await loginForToken(
  99  |     page,
  100 |     "/auth/login",
  101 |     CUSTOMER_CREDENTIALS,
  102 |   );
  103 |   return customerApiToken;
  104 | }
  105 | 
  106 | async function getAdminApiToken(page: Page): Promise<string | null> {
  107 |   adminApiToken ??= await loginForToken(
  108 |     page,
  109 |     "/auth/admin-login",
  110 |     ADMIN_CREDENTIALS,
  111 |   );
  112 |   return adminApiToken;
  113 | }
  114 | 
  115 | export async function ensureNoPendingBookings(page: Page) {
  116 |   try {
  117 |     const apiContext = page.request;
  118 |     const token = await getCustomerApiToken(page);
  119 |     if (!token) return;
  120 | 
  121 |     const bookingsRes = await apiContext.get(
  122 |       `${API_BASE_URL}/bookings?status=Pending`,
  123 |       {
  124 |         headers: { Authorization: `Bearer ${token}` },
  125 |       },
  126 |     );
  127 |     if (bookingsRes.ok()) {
  128 |       const listBody = await bookingsRes.json();
  129 |       const items = listBody.data?.items ?? [];
  130 |       for (const item of items) {
  131 |         await apiContext.post(
  132 |           `${API_BASE_URL}/bookings/${item.bookingId}/cancel`,
  133 |           {
  134 |             headers: { Authorization: `Bearer ${token}` },
  135 |           },
  136 |         );
  137 |       }
  138 |     }
  139 |   } catch {
  140 |     // Non-fatal cleanup
  141 |   }
  142 | }
  143 | 
  144 | export async function loginAsCustomer(page: Page) {
  145 |   await waitForApiReady(page);
  146 |   await ensureNoPendingBookings(page);
  147 | 
  148 |   const MAX_ATTEMPTS = 3;
  149 |   for (let attempt = 1; attempt <= MAX_ATTEMPTS; attempt++) {
  150 |     await page.goto("/auth/login", { waitUntil: "domcontentloaded" });
  151 |     await page.getByLabel("Email").fill(CUSTOMER_CREDENTIALS.email!);
  152 |     await page
  153 |       .getByLabel("Mật khẩu", { exact: true })
  154 |       .fill(CUSTOMER_CREDENTIALS.password!);
  155 |     await page.getByRole("button", { name: "Đăng nhập" }).click();
  156 | 
  157 |     const redirected = await page
  158 |       .waitForURL((url) => !url.pathname.startsWith("/auth/login"), {
  159 |         waitUntil: "commit",
  160 |         timeout: 15_000,
  161 |       })
  162 |       .then(() => true)
  163 |       .catch(() => false);
  164 | 
  165 |     if (redirected) return;
  166 | 
  167 |     if (attempt < MAX_ATTEMPTS) {
  168 |       // Transient DB failure (e.g. Neon cold-start) – wait briefly then retry
  169 |       await page.waitForTimeout(2_000);
  170 |     }
  171 |   }
  172 | 
  173 |   // Final hard assertion so the test fails with a clear message
  174 |   await page.waitForURL((url) => !url.pathname.startsWith("/auth/login"), {
  175 |     waitUntil: "commit",
  176 |     timeout: 15_000,
  177 |   });
  178 | }
  179 | 
  180 | export async function loginAsAdmin(page: Page) {
  181 |   await waitForApiReady(page);
  182 |   const MAX_ATTEMPTS = 3;
  183 |   for (let attempt = 1; attempt <= MAX_ATTEMPTS; attempt++) {
> 184 |     await page.goto("/admin/login", { waitUntil: "domcontentloaded" });
      |                ^ Error: page.goto: Protocol error (Page.navigate): Cannot navigate to invalid URL
  185 |     await page.getByLabel("Email quản trị").fill(ADMIN_CREDENTIALS.email!);
  186 |     await page
  187 |       .getByLabel("Mật khẩu", { exact: true })
  188 |       .fill(ADMIN_CREDENTIALS.password!);
  189 |     await page.getByRole("button", { name: "Đăng nhập quản trị" }).click();
  190 | 
  191 |     const redirected = await page
  192 |       .waitForURL((url) => !url.pathname.startsWith("/admin/login"), {
  193 |         waitUntil: "commit",
  194 |         timeout: 15_000,
  195 |       })
  196 |       .then(() => true)
  197 |       .catch(() => false);
  198 | 
  199 |     if (redirected) return;
  200 | 
  201 |     if (attempt < MAX_ATTEMPTS) {
  202 |       // Transient DB failure (e.g. Neon cold-start) – wait briefly then retry
  203 |       await page.waitForTimeout(2_000);
  204 |     }
  205 |   }
  206 | 
  207 |   // Final hard assertion so the test fails with a clear message
  208 |   await page.waitForURL((url) => !url.pathname.startsWith("/admin/login"), {
  209 |     waitUntil: "commit",
  210 |     timeout: 15_000,
  211 |   });
  212 | }
  213 | 
  214 | export interface TestTicketInfo {
  215 |   ticketId: number;
  216 |   qrCode: string;
  217 |   movieTitle: string;
  218 |   seatRow: string;
  219 |   seatNumber: number;
  220 | }
  221 | 
  222 | export async function getLatestPendingBookingId(
  223 |   page: Page,
  224 |   token: string,
  225 | ): Promise<number | null> {
  226 |   const apiContext = page.request;
  227 |   const bookingsRes = await apiContext.get(
  228 |     `${API_BASE_URL}/bookings?status=Pending`,
  229 |     { headers: { Authorization: `Bearer ${token}` } },
  230 |   );
  231 |   if (!bookingsRes.ok()) return null;
  232 |   const body = await bookingsRes.json();
  233 |   const items = body.data?.items ?? [];
  234 |   if (items.length === 0) return null;
  235 |   return Math.max(...items.map((i: { bookingId: number }) => i.bookingId));
  236 | }
  237 | 
  238 | export async function expireBookingHoldInDb(
  239 |   bookingId: number,
  240 | ): Promise<boolean> {
  241 |   return runSql(
  242 |     `UPDATE "Bookings" SET "ExpiresAt" = (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 minute' WHERE "BookingId" = ${bookingId};`,
  243 |   );
  244 | }
  245 | 
  246 | export async function cancelBookingViaApi(
  247 |   page: Page,
  248 |   token: string,
  249 |   bookingId: number,
  250 | ): Promise<boolean> {
  251 |   const res = await page.request.post(
  252 |     `${API_BASE_URL}/bookings/${bookingId}/cancel`,
  253 |     { headers: { Authorization: `Bearer ${token}` } },
  254 |   );
  255 |   return res.ok();
  256 | }
  257 | 
  258 | export async function holdFirstAvailableSeat(
  259 |   page: Page,
  260 |   token: string,
  261 |   showtimeId = 1,
  262 | ): Promise<{ bookingId: number; bookingCode: string } | null> {
  263 |   const apiContext = page.request;
  264 |   const seatsRes = await apiContext.get(
  265 |     `${API_BASE_URL}/showtimes/${showtimeId}/seats`,
  266 |     { headers: { Authorization: `Bearer ${token}` } },
  267 |   );
  268 |   if (!seatsRes.ok()) return null;
  269 |   const seatsBody = await seatsRes.json();
  270 |   const availableSeat = (seatsBody.data?.seats ?? []).find(
  271 |     (s: { status: string }) => s.status === "Available",
  272 |   );
  273 |   if (!availableSeat) return null;
  274 | 
  275 |   const holdRes = await apiContext.post(`${API_BASE_URL}/bookings/hold`, {
  276 |     headers: { Authorization: `Bearer ${token}` },
  277 |     data: {
  278 |       showtimeId,
  279 |       seatIds: [availableSeat.showtimeSeatId],
  280 |       customerName: "E2E Test User",
  281 |       customerPhone: "0912345678",
  282 |     },
  283 |   });
  284 |   if (!holdRes.ok()) return null;
```