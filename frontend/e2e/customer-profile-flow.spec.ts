import { expect, test } from "@playwright/test";
import { loginAsCustomer } from "./helpers";

test("customer profile flow: view profile, update details, verify persistence, and browse ticket history", async ({
  page,
}) => {
  // 1. Log in as Customer
  await loginAsCustomer(page);

  // 2. Navigate to customer profile
  await page.goto("/account/profile");
  await expect(
    page.getByRole("heading", { name: "Thông tin cá nhân" }),
  ).toBeVisible({ timeout: 10_000 });

  // 3. Update full name and phone number
  const uniqueName = `Khách Hàng Test ${Date.now().toString().slice(-4)}`;
  const fullNameInput = page.getByLabel("Họ và tên");
  await fullNameInput.fill(uniqueName);

  const phoneInput = page.getByLabel("Số điện thoại");
  await phoneInput.fill("0987654321");

  // 4. Submit form and verify success toast
  const saveButton = page.getByRole("button", { name: /Lưu thay đổi/i });
  await saveButton.click();
  await expect(
    page.getByText(/Cập nhật thông tin thành công/i),
  ).toBeVisible({ timeout: 10_000 });

  // 5. Reload page to verify updated profile details persist in database
  await page.reload();
  await expect(fullNameInput).toHaveValue(uniqueName, { timeout: 10_000 });

  // 6. Navigate to Tickets page and verify tabs interaction
  await page.getByRole("link", { name: /Vé đã mua/i }).click();
  await page.waitForURL(/\/account\/tickets/, { timeout: 10_000 });
  await expect(
    page.getByRole("heading", { name: "Vé của tôi" }),
  ).toBeVisible({ timeout: 10_000 });

  // Verify tabs can be clicked and filter states change
  const paidTab = page.getByRole("button", { name: "Đã thanh toán" });
  if (await paidTab.isVisible()) {
    await paidTab.click();
    await expect(paidTab).toHaveClass(/border-primary/);
  }

  const allTab = page.getByRole("button", { name: "Tất cả" });
  await allTab.click();
  await expect(allTab).toHaveClass(/border-primary/);
});
