import { expect, test } from "@playwright/test";
import {
  loginAsCustomer,
  getCustomerApiToken,
  holdFirstAvailableSeat,
  cancelBookingViaApi,
} from "./helpers";

test("customer sees a clear failure state when a booking ends up cancelled/failed, not a fake success", async ({
  page,
}) => {
  await loginAsCustomer(page);

  const token = await getCustomerApiToken(page);
  test.skip(!token, "Could not authenticate as customer via API");

  const hold = await holdFirstAvailableSeat(page, token!);
  test.skip(!hold, "No available seat on showtime 1 to run the scenario");

  const cancelled = await cancelBookingViaApi(page, token!, hold!.bookingId);
  test.skip(
    !cancelled,
    "Could not cancel the booking to simulate a payment failure",
  );

  await page.goto(`/booking/result?bookingId=${hold!.bookingId}`);

  await expect(
    page.getByRole("heading", { name: /Đơn đặt vé đã bị huỷ/i }),
  ).toBeVisible({ timeout: 10_000 });
  await expect(
    page.getByText(/Giao dịch không thành công hoặc đã bị huỷ/i),
  ).toBeVisible();
  await expect(page.getByText(hold!.bookingCode)).toBeVisible();

  await expect(page.locator("main").getByText(/Vé điện tử/i)).not.toBeVisible();

  await expect(
    page.getByRole("link", { name: /Xem vé của tôi/i }),
  ).toBeVisible();
  await expect(page.getByRole("link", { name: /Về trang chủ/i })).toBeVisible();
});

test("visiting the booking result page for a non-existent booking shows a retry state, not a crash", async ({
  page,
}) => {
  await loginAsCustomer(page);

  await page.goto("/booking/result?bookingId=999999999");
  await page.waitForLoadState("networkidle").catch(() => { /* ignore */ });

  await expect(page.getByText(/Không tìm thấy đơn đặt vé này/i)).toBeVisible({
    timeout: 20_000,
  });
  await expect(page.getByRole("button", { name: /Thử lại/i })).toBeVisible();
});
