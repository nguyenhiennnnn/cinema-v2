import { expect, test } from "@playwright/test";
import { createEligibleTicketForCheckin, loginAsAdmin } from "./helpers";

test.setTimeout(120_000);

test("admin ticket check-in flow: reject invalid codes, successfully check-in valid ticket, and prevent duplicate scans", async ({
  page,
}) => {
  // 1. Log in as Admin
  await loginAsAdmin(page);

  // 2. Navigate to Check-in page
  await page.getByRole("link", { name: "Soát vé" }).click();
  await expect(page).toHaveURL(/\/admin\/checkin/, { timeout: 10_000 });
  await expect(
    page.getByRole("heading", { name: /Soát vé & Check-in/i }),
  ).toBeVisible({ timeout: 10_000 });

  // Verify idle status card
  await expect(
    page.getByText("Chưa có vé nào được soát"),
  ).toBeVisible({ timeout: 5_000 });

  const codeInput = page.getByPlaceholder("Quét hoặc nhập mã QR vé");
  const checkinButton = page.getByRole("button", { name: /Soát vé ngay/i });

  // 3. Scenario 1: Attempt check-in with an invalid/non-existent ticket QR code
  await codeInput.fill("INVALID-QR-TEST-99999");
  await checkinButton.click();

  // Verify failure card is rendered
  await expect(
    page.getByText("Soát vé thất bại"),
  ).toBeVisible({ timeout: 10_000 });

  // 4. Scenario 2: Successfully check-in a real confirmed ticket
  const testTicket = await createEligibleTicketForCheckin(page);
  test.skip(!testTicket, "Could not generate a test ticket for check-in");

  if (testTicket) {
    await codeInput.fill(testTicket.qrCode);
    await checkinButton.click();

    // Verify success card is rendered with ticket details
    await expect(
      page.getByText("Check-in thành công"),
    ).toBeVisible({ timeout: 10_000 });
    await expect(
      page.getByText(testTicket.movieTitle).first(),
    ).toBeVisible();
    await expect(
      page.getByText(`${testTicket.seatRow}${testTicket.seatNumber}`).first(),
    ).toBeVisible();

    // 5. Scenario 3: Anti-fraud / Re-scan duplicate check-in attempt
    await codeInput.fill(testTicket.qrCode);
    await checkinButton.click();

    // Verify system rejects duplicate scan
    await expect(
      page.getByText("Soát vé thất bại"),
    ).toBeVisible({ timeout: 10_000 });
    await expect(
      page.getByText(/Vé đã check-in trước đó|đã được sử dụng/i).first(),
    ).toBeVisible({ timeout: 10_000 });
  }
});
