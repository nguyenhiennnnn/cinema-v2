import { expect, test } from "@playwright/test";
import { loginAsCustomer } from "./helpers";

test("customer end-to-end booking flow: select movie, pick seats, add combos, hold, check payment & release", async ({
  page,
}) => {
  // 1. Log in as Customer (also cancels any previous pending bookings)
  await loginAsCustomer(page);

  // 2. Browse movies catalog and pick the first movie
  await page.goto("/movies");
  const firstMovieLink = page.locator('a[href^="/movies/"]').first();
  await expect(firstMovieLink).toBeVisible({ timeout: 10_000 });
  await firstMovieLink.click();
  await page.waitForURL(/\/movies\/\d+/, { timeout: 10_000 });

  // 3. Scroll to showtimes section and select an available showtime slot
  await page.locator("#showtimes").scrollIntoViewIfNeeded();
  const firstShowtimeButton = page
    .locator('#showtimes button[data-slot="showtime-slot"]:not([disabled])')
    .first();
  await expect(firstShowtimeButton).toBeVisible({ timeout: 10_000 });
  await firstShowtimeButton.click();
  await page.waitForURL(/\/booking\/\d+/, { timeout: 10_000 });

  // 4. In seat selection step, select 2 available seats
  const availableSeats = page.locator(
    'button[aria-label^="Ghế "]:not([disabled])',
  );
  await expect(availableSeats.first()).toBeVisible({ timeout: 10_000 });
  const seatCount = await availableSeats.count();
  test.skip(
    seatCount < 2,
    "Not enough available seats on this showtime to run the scenario",
  );

  await availableSeats.nth(0).click();
  await availableSeats.nth(1).click();

  // Verify seat price summary is displayed
  await expect(page.getByText(/Chi tiết đơn hàng/i)).toBeVisible();
  await expect(page.getByText(/Ghế \(2\)/i)).toBeVisible();

  // 5. If combos are available, increase the first combo quantity
  const comboPlusButton = page.locator('button:has(svg.lucide-plus)').first();
  if (await comboPlusButton.isVisible()) {
    await comboPlusButton.click();
    await expect(page.getByText("Combo", { exact: true })).toBeVisible();
  }

  // 6. Hold seats & continue to payment step
  const continueButton = page.getByRole("button", {
    name: /Giữ ghế & tiếp tục thanh toán/i,
  });
  await expect(continueButton).toBeEnabled();
  await continueButton.click();

  // 7. Payment step assertions:
  // Check hold countdown timer is active
  await expect(page.getByText(/Giữ ghế còn/i)).toBeVisible({ timeout: 10_000 });

  // Verify payment action button is visible
  const paymentButton = page.getByRole("button", { name: /^Thanh toán$/i });
  await expect(paymentButton).toBeVisible({ timeout: 10_000 });

  // Verify payment methods can be switched
  const momoButton = page.getByRole("button", { name: "MoMo" });
  if (await momoButton.isVisible()) {
    await momoButton.click();
    await expect(momoButton).toHaveClass(/border-primary/);
  }

  const vnpayButton = page.getByRole("button", { name: "VNPay" });
  if (await vnpayButton.isVisible()) {
    await vnpayButton.click();
    await expect(vnpayButton).toHaveClass(/border-primary/);
  }

  // 8. Cancel hold to release seats back to the showtime
  const cancelButton = page.getByRole("button", { name: /Huỷ, chọn lại ghế/i });
  await expect(cancelButton).toBeVisible();
  await cancelButton.click();

  // Verify returned cleanly to seat selection view
  await expect(continueButton).toBeVisible({ timeout: 10_000 });

  // 9. Navigate to My Tickets to verify account ticket history is accessible
  await page.goto("/account/tickets");
  await expect(page.getByRole("heading", { name: /Vé của tôi/i })).toBeVisible({
    timeout: 10_000,
  });
  await expect(page.getByRole("button", { name: "Tất cả" })).toBeVisible();
});
