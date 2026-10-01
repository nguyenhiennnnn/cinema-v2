import { expect, test } from "@playwright/test";
import { loginAsAdmin } from "./helpers";

test("admin showtimes management flow: switch views, filter, create new showtime, verify, and cancel", async ({
  page,
}) => {
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
  const showtimeRows = page.locator("tbody tr");
  await expect(showtimeRows.first()).toBeVisible({ timeout: 10_000 });

  // 11. Open row action menu to cancel/delete the showtime
  const actionMenuButton = showtimeRows
    .first()
    .getByRole("button", { name: "Mở menu" })
    .first();
  await actionMenuButton.click();

  const cancelMenuItem = page.getByRole("menuitem", {
    name: /Hủy suất chiếu/i,
  });
  await cancelMenuItem.click();

  // Confirm cancel dialog
  const confirmCancelButton = page.getByRole("button", {
    name: /^Hủy suất chiếu$/i,
  });
  await expect(confirmCancelButton).toBeVisible();
  await confirmCancelButton.click();

  // Verify cancel success toast
  await expect(
    page.getByText(/Đã hủy suất chiếu/i),
  ).toBeVisible({ timeout: 10_000 });
});
