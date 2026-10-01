import { expect, test } from "@playwright/test";
import { loginAsAdmin } from "./helpers";

test("admin movie management flow: search, filter, create movie with genres, verify in table, and delete", async ({
  page,
}) => {
  // 1. Log in as Admin
  await loginAsAdmin(page);

  // 2. Navigate to Movie Management
  await page.getByRole("link", { name: "Quản lý phim" }).click();
  await page.waitForURL(/\/admin\/movies/, { timeout: 10_000 });
  await expect(
    page.getByRole("heading", { name: "Quản lý phim" }),
  ).toBeVisible({ timeout: 10_000 });

  // 3. Test search filter on existing movies
  const searchInput = page.getByPlaceholder("Tìm theo tên phim...");
  await expect(searchInput).toBeVisible();
  await searchInput.fill("Oppenheimer");
  await page.waitForTimeout(500); // Debounce
  await expect(page.getByText("Oppenheimer").first()).toBeVisible({
    timeout: 10_000,
  });

  // Clear search to show full list
  await searchInput.clear();
  await page.waitForTimeout(500);

  // 4. Click "Thêm phim" button to open creation Sheet
  const addMovieButton = page.getByRole("button", { name: /Thêm phim/i });
  await addMovieButton.click();

  // Verify Sheet modal is open
  await expect(page.getByText("Nhập thông tin chi tiết của bộ phim.")).toBeVisible({
    timeout: 5_000,
  });

  // 5. Fill movie creation form
  const uniqueTitle = `E2E Phim Mới ${Date.now().toString().slice(-5)}`;
  await page.getByPlaceholder("Nhập tên phim...").fill(uniqueTitle);
  await page.getByLabel(/Thời lượng/i).fill("130");
  await page
    .getByPlaceholder("Tóm tắt nội dung phim...")
    .fill("Đây là mô tả phim được tạo tự động bởi bài kiểm thử E2E.");

  // Select Genre from MultiSelect combobox
  const genreCombobox = page
    .getByRole("combobox")
    .filter({ hasText: "Chọn thể loại phim" });
  await genreCombobox.click();
  const firstGenreOption = page
    .locator('[role="option"], [cmdk-item]')
    .first();
  await expect(firstGenreOption).toBeVisible({ timeout: 5_000 });
  await firstGenreOption.click();
  // Close the genre dropdown by pressing Escape or clicking the label
  await page.keyboard.press("Escape");

  // 6. Submit creation form
  const submitButton = page.locator('button[type="submit"]:has-text("Thêm phim")');
  await submitButton.click();

  // Verify success toast
  await expect(
    page.getByText(/Đã thêm phim mới thành công/i),
  ).toBeVisible({ timeout: 10_000 });

  // 7. Search for newly created movie to verify it appears in DataTable
  await searchInput.fill(uniqueTitle);
  await page.waitForTimeout(600); // Debounce
  const movieRow = page.locator("tr", { hasText: uniqueTitle });
  await expect(movieRow).toBeVisible({ timeout: 10_000 });

  // 8. Open row action menu and delete the newly created movie
  const actionMenuButton = movieRow
    .getByRole("button", { name: "Mở menu" })
    .first();
  await actionMenuButton.click();

  const deleteMenuItem = page.getByRole("menuitem", { name: /Xóa phim/i });
  await deleteMenuItem.click();

  // Confirm in AlertDialog
  const confirmDeleteButton = page.getByRole("button", { name: "Xóa phim" });
  await expect(confirmDeleteButton).toBeVisible();
  await confirmDeleteButton.click();

  // Verify deletion toast
  await expect(
    page.getByText(/Đã xóa phim thành công/i),
  ).toBeVisible({ timeout: 10_000 });

  // Verify movie is removed from table
  await expect(movieRow).not.toBeVisible({ timeout: 10_000 });
});
