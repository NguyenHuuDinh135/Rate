import { expect, test } from '@playwright/test';
import { mkdirSync, writeFileSync } from 'node:fs';
import path from 'node:path';

const outputDir = path.join(process.cwd(), 'artifacts', 'ux-audit');

const routes = [
  { name: 'home', path: '/', heading: /PHIM ĐANG CHIẾU/i },
  { name: 'movies', path: '/movies', heading: /PHIM ĐANG CHIẾU/i },
  { name: 'about', path: '/about', heading: /Đồng Hành Cùng Trải Nghiệm Điện Ảnh Của Bạn/i },
  { name: 'theaters', path: '/theaters', heading: /Hệ Thống Rạp|Rạp/i },
  { name: 'login', path: '/auth/login', heading: /Đăng nhập RATE/i },
  { name: 'register', path: '/auth/register', heading: /Tạo tài khoản/i },
  { name: 'forgot-password', path: '/auth/forgot-password', heading: /Quên mật khẩu/i },
  { name: 'reset-password', path: '/auth/reset-password', heading: /Đặt lại mật khẩu|Mật khẩu/i },
  { name: 'verify-otp', path: '/auth/verify-otp', heading: /Xác thực|OTP|Mã/i },
  { name: 'profile-guest', path: '/profile', heading: /Đăng nhập RATE|Hồ sơ/i },
  { name: 'tickets-guest', path: '/my-tickets', heading: /Đăng nhập RATE|Vé/i },
  { name: 'admin-dashboard-guest', path: '/admin', heading: /Đăng nhập RATE|Dashboard/i },
  { name: 'admin-movies-guest', path: '/admin/movies', heading: /Đăng nhập RATE|Quản lý phim/i },
  { name: 'admin-bookings-guest', path: '/admin/bookings', heading: /Đăng nhập RATE|Đặt vé/i },
  { name: 'admin-payments-guest', path: '/admin/payments', heading: /Đăng nhập RATE|Thanh toán/i },
  { name: 'admin-users-guest', path: '/admin/users', heading: /Đăng nhập RATE|Người dùng/i },
] as const;

const viewports = [
  { name: 'desktop', width: 1440, height: 900 },
  { name: 'mobile', width: 390, height: 844 },
] as const;

type AuditRow = {
  viewport: string;
  route: string;
  finalPath: string;
  status: number | null;
  title: string;
  headings: string[];
  overflowCount: number;
  brokenImageCount: number;
  screenshot: string;
};

test.describe('UX screenshot audit', () => {
  const report: AuditRow[] = [];

  test.beforeAll(() => {
    mkdirSync(outputDir, { recursive: true });
  });

  for (const viewport of viewports) {
    for (const route of routes) {
      test(`${viewport.name} ${route.name}`, async ({ page }) => {
        await page.setViewportSize({ width: viewport.width, height: viewport.height });
        await page.addInitScript(() => localStorage.setItem('rate-theme', 'dark'));

        const response = await page.goto(route.path, { waitUntil: 'networkidle' });
        await expect(page.getByRole('heading', { name: route.heading }).first()).toBeVisible({ timeout: 15_000 });

        const fileName = `${viewport.name}-${route.name}.png`;
        const screenshotPath = path.join(outputDir, fileName);
        await page.screenshot({ path: screenshotPath, fullPage: true });

        const result = await page.evaluate(() => {
          const isVisible = (element: Element) => {
            const style = window.getComputedStyle(element);
            const rect = element.getBoundingClientRect();
            return style.visibility !== 'hidden' && style.display !== 'none' && rect.width > 0 && rect.height > 0;
          };

          const headings = Array.from(document.querySelectorAll('h1,h2,[role="heading"]'))
            .filter(isVisible)
            .map((element) => (element.textContent ?? '').trim())
            .filter(Boolean)
            .slice(0, 8);

          const overflowCount = Array.from(document.querySelectorAll('body *'))
            .filter(isVisible)
            .filter((element) => {
              const rect = element.getBoundingClientRect();
              return rect.left < -1 || rect.right > window.innerWidth + 1;
            }).length;

          const brokenImageCount = Array.from(document.images)
            .filter(isVisible)
            .filter((image) => !image.complete || image.naturalWidth === 0)
            .length;

          return {
            finalPath: window.location.pathname,
            title: document.title,
            headings,
            overflowCount,
            brokenImageCount,
          };
        });

        report.push({
          viewport: viewport.name,
          route: route.path,
          status: response?.status() ?? null,
          screenshot: screenshotPath,
          ...result,
        });

        expect(result.overflowCount).toBe(0);
        expect(result.brokenImageCount).toBe(0);
        await expect(page.locator('#components-reconnect-modal')).toBeHidden();
        await expect(page.locator('#blazor-error-ui')).toBeHidden();
      });
    }
  }

  test.afterAll(() => {
    writeFileSync(path.join(outputDir, 'report.json'), JSON.stringify(report, null, 2));
  });
});
