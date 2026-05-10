import { test, expect } from '@playwright/test';

const BASE_URL = process.env.BASE_URL || 'http://localhost:3000';

test('homepage loads', async ({ page }) => {
  await page.goto(BASE_URL);
  // Check if the page has loaded by checking for a common element or title
  // Since we saw siteConfig.name is "shadcn/ui", we can check for that or just a 200 status.
  const response = await page.request.get(BASE_URL);
  expect(response.ok()).toBeTruthy();
});

test('ai-chat page loads', async ({ page }) => {
  await page.goto(`${BASE_URL}/ai-chat`);
  const response = await page.request.get(`${BASE_URL}/ai-chat`);
  expect(response.ok()).toBeTruthy();
});
