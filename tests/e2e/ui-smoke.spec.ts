import { expect, test } from '@playwright/test';

const publicRoutes = [
  { path: '/', heading: /PHIM ĐANG CHIẾU/i },
  { path: '/movies', heading: /Phim đang chiếu|Phim sắp khởi chiếu/i },
  { path: '/about', heading: /Đồng Hành Cùng Trải Nghiệm Điện Ảnh Của Bạn/i },
  { path: '/auth/login', heading: /Đăng nhập RATE/i },
  { path: '/auth/register', heading: /Tạo tài khoản/i },
  { path: '/auth/forgot-password', heading: /Quên mật khẩu/i },
];

test.describe('public UI smoke', () => {
  for (const route of publicRoutes) {
    test(`${route.path} renders without Blazor crash`, async ({ page }) => {
      await page.addInitScript(() => localStorage.setItem('rate-theme', 'dark'));

      const response = await page.goto(route.path, { waitUntil: 'domcontentloaded' });

      expect(response?.ok()).toBeTruthy();
      await expect(page.getByRole('heading', { name: route.heading }).first()).toBeVisible({ timeout: 15_000 });
      await expect(page.locator('#components-reconnect-modal')).toBeHidden();
      await expect(page.locator('#blazor-error-ui')).toBeHidden();
    });
  }
});

test.describe('admin route aliases', () => {
  const aliases = ['/admin/dashboard', '/admin/cinemas', '/admin/rooms', '/admin/showtimes', '/admin/activity'];

  for (const path of aliases) {
    test(`${path} resolves to an application route`, async ({ page }) => {
      await page.addInitScript(() => localStorage.setItem('rate-theme', 'dark'));

      const response = await page.goto(path, { waitUntil: 'domcontentloaded' });

      expect(response?.ok()).toBeTruthy();
      await expect(page.getByRole('heading', { name: /Đăng nhập RATE|Dashboard|Rạp chiếu|Suất chiếu|Người dùng/i }).first()).toBeVisible({ timeout: 15_000 });
      await expect(page.locator('#components-reconnect-modal')).toBeHidden();
      await expect(page.locator('#blazor-error-ui')).toBeHidden();
    });
  }
});
