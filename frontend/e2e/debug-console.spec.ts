import { test, expect } from "@playwright/test";
import { loginAsCustomer } from "./helpers";

test("debug: check movies page AFTER login", async ({ page }) => {
  const errors: string[] = [];
  const networkReqs: string[] = [];
  
  page.on("console", (msg) => {
    if (msg.type() === "error") errors.push(msg.text());
  });
  
  page.on("pageerror", (err) => {
    errors.push(`PAGE ERROR: ${err.message}`);
  });

  page.on("request", (req) => {
    if (req.url().includes("/movies")) networkReqs.push(`REQ: ${req.url()}`);
  });
  page.on("response", (res) => {
    if (res.url().includes("/movies")) networkReqs.push(`RES ${res.status()}: ${res.url()}`);
  });

  // Login first (like the actual test)
  await loginAsCustomer(page);
  
  await page.goto("/movies");
  
  // Wait up to 10s for content
  const link = page.locator('a[href^="/movies/"]').first();
  const appeared = await link.waitFor({ timeout: 10_000 }).then(() => true).catch(() => false);
  
  console.log("ERRORS:", errors);
  console.log("NETWORK:", networkReqs);
  console.log("Link appeared:", appeared);
  console.log("URL:", page.url());
  
  const mainHtml = await page.locator("main").innerHTML();
  console.log("MAIN (500 chars):", mainHtml.substring(0, 500));
  
  expect(appeared).toBe(true);
});
