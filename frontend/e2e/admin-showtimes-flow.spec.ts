import { expect, test } from "@playwright/test";
import { cleanFutureTestShowtimes, loginAsAdmin } from "./helpers";

test.setTimeout(120_000);

test("admin showtimes management flow: switch views, filter, create new showtime, verify, and cancel", async ({
  page,
}) => {
  // Ensure a clean slate for future test showtimes
  await cleanFutureTestShowtimes();

  // 1. Log in as Admin
  await loginAsAdmin(page);

  // 2. Navigate to Showtimes page
  await page.getByRole("link", { name: "Suất chiếu" }).click();
  await page.waitForURL(/\/admin\/showtimes/, { timeout: 10_000 });
  await expect(
    page.getByRole("heading", { name: "Quản lý Suất chiếu" }),
  ).toBeVisible({ timeout: 10_000 });

  // 3. Test tab navigation between List and Calendar
  const calendarTab = page.getByRole("tab", { name: "Lịch" });
  await calendarTab.click();
  await expect(
    page.getByRole("tabpanel", { name: "Lịch" }),
  ).toBeVisible({ timeout: 5_000 });

  const listTab = page.getByRole("tab", { name: "Danh sách" });
  await listTab.click();
  await expect(page.locator("table")).toBeVisible({ timeout: 10_000 });

  // 4. Click "Thêm suất chiếu" to open creation dialog
  const addShowtimeButton = page.getByRole("button", {
    name: /Thêm suất chiếu/i,
  });
  await addShowtimeButton.click();

  // Verify Dialog is open
  await expect(
    page.getByRole("heading", { name: "Tạo suất chiếu mới" }),
  ).toBeVisible({ timeout: 5_000 });

  // 5. Select Movie
  const movieCombobox = page
    .getByRole("combobox")
    .filter({ hasText: "Chọn phim" });
  await movieCombobox.click();
  const firstMovieOption = page.locator('[role="option"], [cmdk-item]').first();
  await expect(firstMovieOption).toBeVisible({ timeout: 5_000 });
  const selectedMovieTitle =
    (await firstMovieOption.textContent())?.trim() ?? "";
  await firstMovieOption.click();

  // 6. Select Room
  const roomCombobox = page
    .getByRole("combobox")
    .filter({ hasText: "Chọn phòng chiếu" });
  await roomCombobox.click();
  const firstRoomOption = page.locator('[role="option"], [cmdk-item]').first();
  await expect(firstRoomOption).toBeVisible({ timeout: 5_000 });
  await firstRoomOption.click();

  // 7. Pick Date & Time (Select an available day in calendar)
  const datePickerTrigger = page.locator(
    'button:has(svg.lucide-calendar-clock)',
  );
  await datePickerTrigger.click();

  // Pick a date (e.g. today or last enabled day)
  const calendarDay = page
    .locator('[role="gridcell"] button:not([disabled])')
    .last();
  await expect(calendarDay).toBeVisible({ timeout: 5_000 });
  await calendarDay.click();

  // Ensure a valid daytime hour (10:00) within cinema operating hours (08:00 - 23:59)
  const timeInput = page.locator('input[type="time"]');
  await expect(timeInput).toBeEnabled({ timeout: 5_000 });
  await timeInput.fill("10:00");

  // Click "Xong" in datetime popover
  const doneButton = page.getByRole("button", { name: "Xong" });
  await expect(doneButton).toBeEnabled();
  await doneButton.click();

  // 8. Set Base Price
  const priceInput = page.getByLabel(/Giá vé/i);
  await priceInput.fill("95000");

  // 9. Submit showtime creation
  const submitButton = page.getByRole("button", {
    name: /^Tạo suất chiếu$/i,
  });
  await submitButton.click();

  // Verify success toast
  await expect(
    page.getByText(/Đã tạo suất chiếu mới/i),
  ).toBeVisible({ timeout: 10_000 });

  // 10. Verify created showtime appears in DataTable
  const showtimeRow = page
    .locator("tbody tr")
    .filter({ hasText: selectedMovieTitle })
    .filter({ has: page.getByRole("button", { name: "Mở menu" }) })
    .first();
  await expect(showtimeRow).toBeVisible({ timeout: 10_000 });

  // 11. Open row action menu to cancel/delete the showtime
  const actionMenuButton = showtimeRow
    .getByRole("button", { name: "Mở menu" })
    .first();
  await actionMenuButton.click();

  const cancelMenuItem = page.getByRole("menuitem", {
    name: /Hủy suất chiếu/i,
  });
  await cancelMenuItem.click();

  // Confirm cancel dialog
  const alertDialog = page.getByRole("alertdialog");
  await expect(alertDialog).toBeVisible({ timeout: 5_000 });
  const confirmCancelButton = alertDialog.getByRole("button", {
    name: /Hủy suất chiếu/i,
  });
  await expect(confirmCancelButton).toBeVisible();
  await confirmCancelButton.click();

  // Verify cancel success toast and dialog dismissed
  await expect(alertDialog).not.toBeVisible({ timeout: 10_000 });
  await expect(
    page.getByText(/Đã h[uủ][yỷ] suất chiếu/i),
  ).toBeVisible({ timeout: 10_000 });
});
