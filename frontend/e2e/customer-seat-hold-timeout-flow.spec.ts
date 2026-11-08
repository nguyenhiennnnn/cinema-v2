import { expect, test } from "@playwright/test";
import {
  loginAsCustomer,
  getCustomerApiToken,
  getLatestPendingBookingId,
  expireBookingHoldInDb,
} from "./helpers";

test.setTimeout(180_000);

test("seat hold that expires mid-checkout notifies the customer and returns them to seat selection", async ({
  page,
}) => {
  await loginAsCustomer(page);

  await page.goto("/movies");
  const firstMovieLink = page.locator('a[href^="/movies/"]').first();
  await expect(firstMovieLink).toBeVisible({ timeout: 10_000 });
  await firstMovieLink.click();
  await expect(page).toHaveURL(/\/movies\/\d+/, { timeout: 10_000 });

  await page.locator("#showtimes").scrollIntoViewIfNeeded();
  const firstShowtimeButton = page
    .locator('#showtimes button[data-slot="showtime-slot"]:not([disabled])')
    .first();
  await expect(firstShowtimeButton).toBeVisible({ timeout: 10_000 });
  await firstShowtimeButton.click();
  await expect(page).toHaveURL(/\/booking\/\d+/, { timeout: 10_000 });

  const availableSeats = page.locator(
    'button[aria-label^="Ghế "]:not([disabled])',
  );
  await expect(availableSeats.first()).toBeVisible({ timeout: 10_000 });
  test.skip(
    (await availableSeats.count()) < 1,
    "No available seats on this showtime to run the scenario",
  );
  await availableSeats.first().click();

  const continueButton = page.getByRole("button", {
    name: /Giữ ghế & tiếp tục thanh toán/i,
  });
  // Wait up to 10 s for the button to become enabled: the authenticated user's
  // display name is fetched asynchronously (auth store hydration), and canHold
  // stays false until customerName is populated from authUser.fullName.
  await expect(continueButton).toBeEnabled({ timeout: 10_000 });
  await continueButton.click();

  await expect(page.getByText(/Giữ ghế còn/i)).toBeVisible({ timeout: 10_000 });

  const token = await getCustomerApiToken(page);
  test.skip(!token, "Could not authenticate as customer via API");
  const bookingId = await getLatestPendingBookingId(page, token!);
  test.skip(
    !bookingId,
    "Could not resolve the pending booking created by the hold",
  );

  const forced = await expireBookingHoldInDb(bookingId!);
  test.skip(
    !forced,
    "Could not reach the database container to force-expire the hold",
  );

  await expect(
    page.getByText(/Thời gian giữ ghế đã hết hạn, vui lòng chọn lại\./i),
  ).toBeVisible({ timeout: 120_000 });

  await expect(continueButton).toBeVisible({ timeout: 10_000 });
  await expect(page.getByText(/Giữ ghế còn/i)).not.toBeVisible();
});
