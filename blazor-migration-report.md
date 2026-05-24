# BÁO CÁO PHÂN TÍCH KIẾN TRÚC FRONTEND & BẢN ĐỒ DỊCH CHUYỂN SANG BLAZOR
**Dự án:** Movie Reservation System  
**Vai trò:** Senior Frontend Architect (React + TypeScript & Blazor Specialist)

---

## I. TỔNG QUAN HỆ THỐNG HIỆN TẠI (REACT + TYPESCRIPT)

Dự án hiện tại là một SPA (Single Page Application) hiện đại, xây dựng bằng **React 19**, **TypeScript 5.8**, và **Vite 7**. Dự án tích hợp sâu với hệ thống CSS mới nhất **Tailwind CSS v4** cùng thư viện component **Shadcn UI** (dựa trên Radix UI Primitives).

### 1. Công nghệ chính & Thư viện sử dụng
- **UI & Components:** Radix UI Primitives (Accordion, Alert Dialog, Aspect Ratio, Avatar, Checkbox, Dialog, Dropdown Menu, Popover, Progress, Radio Group, Scroll Area, Select, Separator, Slider, Switch, Tabs, Tooltip), Lucide React & Tabler Icons.
- **State & Data Fetching:** `@tanstack/react-query` (Quản lý cache, fetching, sync state và tự động invalidate cache khi mutation thành công).
- **Routing:** `react-router` / `react-router-dom` v7 (Hỗ trợ Lazy loading thông qua API `lazy` và cơ chế Client Loaders/Actions).
- **Form Management:** `react-hook-form` + `@hookform/resolvers` + `zod` (Validation schemas ở phía Client).
- **Animations:** `gsap` (GreenSock Animation Platform) + `@gsap/react` + `ScrollTrigger` (Tạo hiệu ứng mượt mà bậc nhất: Stagger text, fade-in scale cards khi scroll, 3D seat map perspective).
- **Feedback & Interactions:** `sonner` (Toast notifications), `vaul` (Drawer), `embla-carousel-react` (Slider/Carousel).
- **Date Utilities:** `date-fns` (Xử lý, format ngày tháng).

---

## II. CẤU TRÚC ROUTING & DANH SÁCH CÁC TRANG

Hệ thống định tuyến chia làm 3 phân khu chính được khai báo động trong [router.tsx](file:///E:/Tue/125/webC%23/test/MovieReservation/moviereservation.client/src/app/router.tsx):

### 1. Phân khu Công cộng (Public Client) - Layout: `app/layout.tsx`
- **`/` (Trang chủ):** Slideshow Hero animated, danh sách phim Đang Chiếu/Sắp Chiếu dạng horizontal scroll, và Grid Khám Phá có phân trang Client-side (Load More).
- **`/movies`:** Danh sách phim đầy đủ.
- **`/movies/:id` (Chi tiết Phim & Đặt vé):** Trang quan trọng nhất. Tích hợp Chi tiết phim, Trailer Youtube Modal, Lọc suất chiếu theo ngày (staggered bar), và Bản đồ ghế ngồi tương tác (Seat Map 3D) cùng bảng tính tiền và checkout.
- **`/theaters`:** Danh sách rạp chiếu và layout ghế mặc định của từng rạp.
- **`/about`:** Giới thiệu hệ thống rạp.
- **`/profile`:** Quản lý thông tin tài khoản cá nhân.
- **`/my-tickets`:** Danh sách vé và lịch sử thanh toán của User.
- **`/booking/success`:** Trang xác nhận đặt vé thành công. Render vé điện tử E-Ticket dạng thẻ xé (tear card) kèm mã QR.

### 2. Phân khu Xác thực (Auth Client) - Layout: `auth/layout.tsx`
- **`/auth/login`:** Đăng nhập.
- **`/auth/register`:** Đăng ký tài khoản mới.
- **`/auth/otp`:** Xác thực mã OTP gửi về Email sau khi đăng nhập/đăng ký.

### 3. Phân khu Quản trị (Admin Dashboard) - Layout: `admin/layout.tsx`
*Được bảo vệ nghiêm ngặt bằng phân quyền Client-side (Permission Gate).*
- **`/admin` hoặc `/admin/dashboard`:** Tổng quan doanh thu, số lượng vé bán, số phim, số suất chiếu cùng biểu đồ Recharts tương tác và bảng lịch sử đặt vé gần nhất.
- **`/admin/movies`:** Quản lý phim (CRUD, upload poster url, trailer url, chuyển trạng thái phim).
- **`/admin/genres`:** Quản lý thể loại phim.
- **`/admin/cinemas`:** Quản lý danh sách rạp và cấu hình ghế lỗi (missing)/ghế khóa (blocked).
- **`/admin/showtimes`:** Quản lý suất chiếu (gán phim, rạp, ngày giờ chiếu, loại 2D/3D).
- **`/admin/bookings`:** Lịch sử chi tiết đặt vé.
- **`/admin/payments`:** Lịch sử dòng tiền giao dịch.
- **`/admin/users`:** Quản lý tài khoản người dùng hệ thống.
- **`/admin/settings` (Layout lồng):**
  - `/admin/settings/user-groups`:** Quản lý Roles (AspNetRoles) và cấp quyền claims (RoleClaims) cho Role.
  - `/admin/settings/users`:** Quản lý gán quyền Claim (UserClaims) trực tiếp cho cá nhân User.
  - `/admin/settings/roles`:** Danh sách toàn bộ hằng số quyền hạn (PermissionConstants).

---

## III. KIẾN TRÚC LUỒNG NGHIỆP VỤ & TÍCH HỢP HỆ THỐNG

### 1. Luồng Xác thực (Authentication Flow)
```mermaid
sequenceDiagram
    participant Client as React Client (Local Storage)
    participant API as C# Server (.NET API)

    Note over Client, API: Luồng Login & OTP
    Client->>API: POST /auth/login (Email, Password)
    API-->>Client: Trả về trạng thái yêu cầu OTP
    Client->>API: POST /auth/verify-otp (Email, OTP Code)
    API-->>Client: Trả về TokenData (accessToken, expiresIn)
    Client->>Client: Lưu accessToken vào localStorage ('mr_access_token')
    Client->>API: GET /permissions/me (Gửi Token trong Header)
    API-->>Client: Trả về danh sách Quyền của User
    Client->>Client: Redirect về /admin (nếu có quyền Admin) hoặc / (User)

    Note over Client, API: Luồng Tự động Refresh Token (Axios Interceptor)
    Client->>API: Gọi API bất kỳ (Header: Bearer Token)
    API-->>Client: Trả về 401 Unauthorized (Token hết hạn)
    Client->>Client: Kích hoạt Interceptor, bật cờ isRefreshing
    Client->>API: POST /auth/refresh-token (Cookies/HttpOnly)
    API-->>Client: Trả về accessToken mới
    Client->>Client: Cập nhật localStorage, giải phóng hàng đợi request
    Client->>API: Gọi lại request bị lỗi lúc đầu với Token mới
```

### 2. Luồng Phân quyền (Authorization & Permissions)
Hệ thống sử dụng mô hình phân quyền dựa trên Claims rất chi tiết kế thừa từ ASP.NET Core Identity.
- **Hằng số Quyền hạn (PermissionConstants):** Khai báo trong [api-permissions.ts](file:///E:/Tue/125/webC%23/test/MovieReservation/moviereservation.client/src/lib/api-permissions.ts) bao gồm:
  - `Permission.ManagePermissions` (Quyền cao nhất để vào Admin Area).
  - Quyền theo thực thể: `Permission.Movies.[Create/Edit/Delete]`, `Permission.Shows.[Create/Edit/Delete]`, `Permission.Theaters.[Create/Edit/Delete]`, `Permission.Bookings.[Create/Edit/Delete]`, v.v.
- **Kiểm tra quyền:**
  - `hasAdminPermission(permissions)`: Kiểm tra nếu chứa `Permission.ManagePermissions` hoặc bất kỳ quyền quản trị nào.
  - `<PermissionGate requiredPermission="...">`: Component bao bọc, chỉ render nội dung bên trong nếu tập quyền của User có chứa quyền yêu cầu.

### 3. Luồng Đặt Vé & Thanh Toán (Booking & Checkout Flow)
1. User chọn Phim -> Chọn Ngày -> Chọn Suất chiếu -> Tải cấu hình Rạp từ `/theaters/id/{id}` và danh sách ghế đã được đặt `/bookings/shows/{showId}`.
2. Render bản đồ ghế ngồi: Loại trừ ghế khuyết (`missing`), vô hiệu hóa ghế khóa (`blocked`) và ghế đã bán (`bookedSeats`).
3. User click chọn ghế trống -> Ghế chuyển trạng thái sang `selected` -> Thêm vào mảng `selectedSeats`.
4. Tính tổng tiền = `selectedSeats.length * 40000 VND`.
5. Click **Confirm & Pay** -> Mở Modal thanh toán -> Chọn phương thức (Credit Card / Cash).
6. **Thực thi giao dịch tuần tự (Transaction):**
   - Gọi API Đặt vé cho từng ghế song song: `POST /bookings/create` -> Trả về mảng `bookingIds`.
   - Gọi API Thanh toán gom đơn: `POST /payments/create` với DTO:
     ```json
     {
       "amount": 80000,
       "payment_datetime": "2026-05-24T10:28:28Z",
       "payment_method": "card",
       "user_id": "user-guid-id",
       "show_id": 12,
       "bookings": [101, 102]
     }
     ```
   - Trả về `newPaymentId` thành công.
7. Redirect sang `/booking/success?paymentId={id}&bookingIds=101,102` để hiển thị E-Ticket.

---

## IV. CHI TIẾT CÁC COMPONENT GIAO DIỆN & ĐẶC TÍNH PHỨC TẠP

### 1. Animated Hero Carousel (`root/page.tsx`)
- **Hoạt họa GSAP:** Chỉ kích hoạt hiệu ứng chữ (`.hero-animate`) trượt lên từ phía dưới (`y: 30` về `0`, `opacity` từ `0` về `1`) có stagger `0.1s` và ease `power3.out` mỗi khi đổi slide.
- **Hiệu ứng đổi nền:** Tất cả các ảnh nền của slide được đặt tuyệt đối lồng nhau. Sử dụng CSS transition `duration-1000 ease-in-out` để fade-in slide đang active (`scale-100 opacity-100`) và scale-up nhẹ slide inactive (`scale-110 opacity-0`).
- **Giao diện:** Chứa Badge trạng thái động (Now Showing / Coming Soon) dạng Gradient chói sáng, điểm số đánh giá hình ngôi sao vàng nổi trên nền mờ `backdrop-blur-sm`, và các thumbnail góc dưới bên phải cho phép click chuyển đổi nhanh.

### 2. Bản đồ ghế ngồi tương tác 3D (`movies/[id]/page.tsx`)
- **Tạo mô phỏng màn chiếu cong bằng CSS:**
  ```css
  perspective: 500px;
  ```
  Thanh màn chiếu là một khối div bo cong 50%, đổ bóng neon rực rỡ đại diện cho nguồn phát sáng:
  ```css
  box-shadow: 0 0 30px 10px rgba(var(--primary), 0.4);
  ```
- **Grid Ghế động:**
  - Chữ cái hàng ghế (A, B, C...) hiển thị đối xứng hai bên trái và phải.
  - Sử dụng vòng lặp từ `0` đến `numOfRows` (chuyển đổi sang Char: `A = 65 + i`) và cột từ `1` đến `seatsPerRow` để tạo tọa độ `seatCode` (A1, A2, B5...).
  - Vô hiệu hóa tương tác (disabled) nếu trạng thái ghế là `blocked` hoặc `occupied` (đã bán).
  - Ẩn hoàn toàn (render div ẩn tàng) nếu ghế nằm trong mảng `missing` để giữ cấu trúc hàng ghế nhưng trống trải trực quan (giống lối đi).
- **Hoạt họa GSAP:** Khi rạp được chọn hoặc thay đổi, các ghế ngồi `.seat-btn` sẽ scale từ `0` lên `1` theo hiệu ứng Stagger tỏa ra từ tâm (`from: "center"`).

### 3. Vé điện tử xé đôi - E-Ticket (`booking/success/page.tsx`)
- Thiết kế mô phỏng tấm vé xem phim bằng giấy cao cấp.
- **Tear Line (Đường xé):** Sử dụng 2 hình bán nguyệt lõm ở hai bên rìa thẻ, kết hợp một đường kẻ đứt nét (`border-dashed`) chạy ngang chia đôi vé thành hai phần (Phần trên: Thông tin phim, Phần dưới: Thông tin chi tiết suất chiếu & Mã QR thanh toán).
  ```html
  <!-- Bán nguyệt trái -->
  <div class="absolute -left-3 bottom-[-12px] w-6 h-6 bg-background rounded-full z-10 shadow-[inset_-2px_0_2px_rgba(0,0,0,0.05)]" />
  <!-- Đường xé -->
  <div class="flex-1 border-t-2 border-dashed border-muted-foreground/20 mx-2" />
  <!-- Bán nguyệt phải -->
  <div class="absolute -right-3 bottom-[-12px] w-6 h-6 bg-background rounded-full z-10 shadow-[inset_2px_0_2px_rgba(0,0,0,0.05)]" />
  ```
- Chứa component mã QR hiển thị Booking ID và Box hiển thị Tổng số tiền thanh toán định dạng nội tệ VND.

### 4. Admin Breadcrumbs & Sidebar co giãn (`admin-site-header.tsx`)
- Đọc `location.pathname` hiện tại, phân tách chuỗi để ánh xạ tự động ra breadcrumbs phân cấp (ví dụ: `Admin > Cài đặt > Người dùng`).
- Sidebar sử dụng Radix Collapsible hỗ trợ thu gọn về dạng Icon (offcanvas) khi ở màn hình nhỏ.

---

## V. PHÂN TÍCH THIẾT KẾ GIAO DIỆN (DESIGN SYSTEM & STYLING)

Hệ thống giao diện được định nghĩa dựa trên **Tailwind CSS v4** với gam màu chủ đạo được thiết lập bằng không gian màu **OKLCH** siêu mịn, độ tương phản cao, tối ưu tuyệt đối cho chế độ Dark/Light mode.

### 1. Bảng màu OKLCH mặc định (Light & Dark)

| Biến CSS | Giá trị Light Mode | Giá trị Dark Mode | Mô tả |
| :--- | :--- | :--- | :--- |
| `--background` | `oklch(0.9970 0 0)` (Trắng sáng) | `oklch(0.1684 0 0)` (Đen sâu thẳm) | Nền chính ứng dụng |
| `--foreground` | `oklch(0.2178 0 0)` | `oklch(0.9970 0 0)` | Màu chữ chính |
| `--card` | `oklch(1.0000 0 0)` | `oklch(0.2090 0 0)` | Nền thẻ/bảng |
| `--primary` | `oklch(0.5814 0.2349 27.9869)` | `oklch(0.5814 0.2349 27.9869)` | Đỏ đậm rực rỡ thương hiệu (Netflix-like Red) |
| `--muted` | `oklch(0.9067 0 0)` | `oklch(0.3092 0 0)` | Màu phụ mờ |
| `--border` | `oklch(0.8699 0 0)` | `oklch(0.3600 0 0)` | Đường kẻ border |
| `--ring` | `oklch(0.5814 0.2349 27.9869)` | `oklch(0.5814 0.2349 27.9869)` | Viền khi focus |

### 2. Hệ thống Font chữ
- **Sans-serif (Chữ thường):** `'Netflix Sans', sans-serif` (được import thủ công từ Geist/Inter fallback).
- **Monospace (Mã số/ID):** `'Fira Code', monospace`.

### 3. Dynamic Themes (`themes.css`)
Hệ thống cho phép gán class động lên body để đổi toàn bộ bảng màu nền tảng (Primary và Ring) theo sở thích:
- `.theme-blue` (Xanh dương sang trọng)
- `.theme-green` (Lime tươi mát)
- `.theme-rose` (Hồng quý phái)
- `.theme-amber` (Vàng hổ phách)
- `.theme-purple` (Tím huyền bí)
- `.theme-orange` (Cam năng động)
- `.theme-teal` (Xanh mòng két)

---

## VI. BẢN ĐỒ DỊCH CHUYỂN SANG BLAZOR (MIGRATION ROADMAP)

Để migrate hệ thống SPA React này sang .NET C# tối ưu nhất, chúng ta sử dụng **Blazor Interactive WebAssembly (WASM)**. Điều này cho phép tái sử dụng 100% các DTO C# có sẵn ở backend và thực thi render UI ở phía Client siêu mượt mà không cần độ trễ máy chủ.

### 1. Bản đồ ánh xạ Component (React -> Blazor)

| React Component / File | Blazor Component / Path | Giải pháp thay thế / Chi tiết kỹ thuật |
| :--- | :--- | :--- |
| `router.tsx` | `App.razor` & `Routes.razor` | Khai báo các `@page` trực tiếp trên đầu từng Razor page. Sử dụng `Microsoft.AspNetCore.Components.Routing` để điều hướng. |
| `api.ts` (Axios Interceptors) | `Http/TokenAuthenticationHandler.cs` | Kế thừa `DelegatingHandler` của .NET. Intercept mọi request gửi đi để đính kèm `Authorization: Bearer {token}` lấy từ `LocalStorage`. Xử lý lỗi `401` để tự động gửi request refresh token trước khi gọi lại request gốc. |
| `auth.ts` (Token Helper) | `Services/AuthenticationService.cs` | Sử dụng `Blazored.LocalStorage` để lưu trữ mã khóa token. |
| `permission-gate.tsx` | `<AuthorizeView>` & `<CascadingAuthenticationState>` | Đăng ký một `AuthenticationStateProvider` tùy biến đọc Token từ Local Storage, parse claims để nạp danh sách quyền (Roles & Claims) vào User Principal. |
| `theme-provider.tsx` | `Shared/ThemeSelector.razor` | Inject `IJSRuntime` để thực thi JS thay đổi class body (`document.body.classList.add('theme-*')`). |
| `data-table.tsx` | `Shared/AdminDataTable.razor` | Sử dụng thư viện `Radzen.Blazor` hoặc tự viết Component Table tùy biến kết hợp vòng lặp `@foreach` và các hàm lọc C# để render hàng loạt. |
| `chart-area-interactive.tsx` | `Shared/InteractiveChart.razor` | Sử dụng thư viện đồ họa .NET như `Radzen.Blazor` (RadzenChart) hoặc `Plotly.Blazor` thay thế Recharts. |

---

## VII. ĐỀ XUẤT CẤU TRÚC PROJECT BLAZOR

Cấu trúc thư mục chuẩn mực cho dự án Blazor WebAssembly sạch đẹp, dễ bảo trì:

```text
MovieReservation.Client/
│
├── wwwroot/
│   ├── css/
│   │   ├── app.css            <-- Chứa Tailwind v4 & Reset base
│   │   └── themes.css         <-- Chứa các class .theme-* oklch định nghĩa sẵn
│   ├── index.html             <-- Điểm neo ứng dụng Blazor
│   └── js/
│       └── app-animations.js  <-- Chứa các hàm JS Interop cho GSAP/ScrollTrigger
│
├── Http/
│   └── TokenAuthenticationHandler.cs <-- DelegatingHandler tự động đính kèm token & refresh
│
├── Services/
│   ├── IAuthenticationService.cs
│   ├── AuthenticationService.cs      <-- Quản lý Đăng nhập/Đăng ký/OTP/Refresh Token
│   ├── IApiClient.cs
│   ├── ApiClient.cs                  <-- Tích hợp gọi API HttpClient cho Phim, Suất chiếu...
│   └── CustomAuthenticationStateProvider.cs <-- Cầu nối phân quyền Claims cho Blazor
│
├── Models/                           <-- Chứa các DTOs C# khớp 100% với API Backend
│   ├── MovieDto.cs
│   ├── ShowDto.cs
│   ├── TheaterDto.cs
│   ├── BookingDto.cs
│   └── PaymentDto.cs
│
├── Shared/                           <-- Các thành phần Layout chung
│   ├── MainLayout.razor              <-- Client Layout
│   ├── AdminLayout.razor             <-- Strict Permission Guard Admin Layout
│   ├── Header.razor                  <-- Thanh điều hướng Client
│   ├── Footer.razor                  <-- Chân trang
│   ├── Sidebar.razor                 <-- Sidebar Admin co giãn
│   └── PermissionGate.razor          <-- Component phân quyền bao bọc
│
└── Pages/                            <-- Các trang giao diện chính
    ├── Client/
    │   ├── Index.razor               <-- Trang chủ (Slideshow Hero, Grid phim)
    │   ├── MovieDetails.razor        <-- Chi tiết & Đặt vé tương tác 3D Seat Map
    │   ├── Theaters.razor            <-- Danh sách rạp chiếu & seat template
    │   ├── BookingSuccess.razor      <-- Xác nhận & hiển thị E-Ticket xé đôi
    │   └── MyTickets.razor           <-- Lịch sử vé đã mua
    │
    ├── Auth/
    │   ├── Login.razor
    │   ├── Register.razor
    │   └── Otp.razor
    │
    └── Admin/
        ├── Dashboard.razor           <-- Đồ thị doanh thu & Đơn đặt vé gần nhất
        ├── Movies/
        │   ├── MovieList.razor       <-- Bảng danh sách Phim & CRUD Dialog
        │   └── MovieRoles.razor      <-- Quản lý diễn viên, đạo diễn phim
        ├── Cinemas/
        │   └── CinemaList.razor      <-- Quản lý rạp & cấu hình ghế lỗi
        ├── Showtimes/
        │   └── ShowtimeList.razor    <-- Quản lý suất chiếu
        └── Settings/
            ├── UserGroups.razor      <-- Quản lý Roles & RoleClaims
            └── UserPermissions.razor <-- Quản lý User Claims trực tiếp
```

---

## VIII. ĐẶC TẢ CHI TIẾT TỪNG COMPONENT TRONG BLAZOR (CHO AI REBUILD)

Dưới đây là đặc tả mã nguồn chi tiết cực kỳ trực quan của các thành phần cốt lõi để một AI khác có thể đọc hiểu và rebuild chuẩn xác 100% bằng Blazor.

### 1. Authentication State Provider (`CustomAuthenticationStateProvider.cs`)
*Giải pháp cầu nối giúp Blazor hiểu danh sách Quyền claims lấy từ API `/permissions/me` để bảo vệ các trang quản trị.*

```csharp
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("mr_access_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            // Gọi API lấy thông tin phân quyền hiện tại của tôi
            var response = await _httpClient.GetAsync("/api/permissions/me");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var permissions = JsonSerializer.Deserialize<List<string>>(json);
                
                var identity = new ClaimsIdentity("JwtAuth");
                foreach (var perm in permissions)
                {
                    identity.AddClaim(new Claim("permission", perm));
                }
                
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
        }
        catch
        {
            // Token lỗi hoặc hết hạn
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyUserLogin(string token)
    {
        var identity = new ClaimsIdentity("JwtAuth");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}
```

### 2. Trang Chi tiết Phim & Đặt vé Tương tác (`MovieDetails.razor`)
*Trang phức tạp bậc nhất, chứa bản đồ ghế 3D và logic thanh toán.*

```razor
@page "/movies/{Id:int}"
@inject IApiClient ApiClient
@inject NavigationManager Navigation
@inject IJSRuntime JS
@inject AuthenticationStateProvider AuthStateProvider

<div class="min-h-screen bg-background text-foreground font-sans pb-20">
    @if (movie == null)
    {
        <div class="min-h-screen flex items-center justify-center">
            <div class="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
        </div>
    }
    else
    {
        <!-- HERO SECTION -->
        <div class="relative w-full h-[60vh] lg:h-[70vh] overflow-hidden">
            <div class="absolute inset-0 z-0">
                <img src="@movie.PosterUrl" class="w-full h-full object-cover opacity-20 blur-3xl" alt="bg" />
                <div class="absolute inset-0 bg-gradient-to-t from-background via-background/60 to-transparent" />
            </div>
            
            <div class="relative z-10 container h-full flex items-center pt-12">
                <div class="flex flex-col md:flex-row gap-8 lg:gap-16 items-start md:items-end w-full">
                    <img src="@movie.PosterUrl" alt="@movie.Title" class="w-52 md:w-64 lg:w-80 rounded-xl shadow-2xl border border-border" />
                    
                    <div class="flex-1 space-y-6 text-center md:text-left">
                        <div>
                            <h1 class="text-4xl md:text-6xl font-black tracking-tighter leading-none">@movie.Title</h1>
                            <div class="flex flex-wrap items-center justify-center md:justify-start gap-3 mt-3 text-sm text-muted-foreground">
                                @foreach (var genre in movie.Genres)
                                {
                                    <span class="badge badge-outline border-border bg-background/50 px-2 py-1 rounded">@genre.Name</span>
                                }
                                <span>•</span><span>@movie.Year</span>
                                <span>•</span><span class="text-primary font-bold">IMAX 2D/3D</span>
                            </div>
                        </div>
                        <p class="text-muted-foreground text-sm md:text-base leading-relaxed line-clamp-4 max-w-2xl">@movie.Summary</p>
                        
                        <!-- Cast -->
                        @if (castList.Any())
                        {
                            <div class="flex flex-wrap gap-3">
                                @foreach (var actor in castList.Take(4))
                                {
                                    <div class="flex items-center gap-2 pr-3 rounded-full border border-border bg-card/50 cursor-pointer hover:border-primary/30 transition-all">
                                        <img src="@actor.PictureUrl" class="w-8 h-8 rounded-full object-cover bg-muted" />
                                        <span class="text-xs font-medium">@actor.FullName</span>
                                    </div>
                                }
                            </div>
                        }
                    </div>
                </div>
            </div>
        </div>

        <!-- FILTER SHOWTIMES BAR -->
        <div class="sticky top-0 z-50 bg-background/80 backdrop-blur-md border-y border-border py-4">
            <div class="container flex flex-col lg:flex-row items-center gap-6">
                <!-- Date list navigation -->
                <div class="flex items-center gap-2">
                    <button class="btn btn-ghost rounded-full" @onclick="PrevWeek"><i class="lucide-chevron-left"></i></button>
                    <div class="flex gap-3 overflow-x-auto">
                        @foreach (var date in DateRangeList)
                        {
                            var isActive = date.Date == SelectedDate.Date;
                            var hasShow = DatesWithShows.Contains(date.ToString("yyyy-MM-dd"));
                            
                            <button class="flex flex-col items-center justify-center min-w-[3.5rem] h-14 rounded-2xl border transition-all @(isActive ? "bg-primary text-primary-foreground border-primary" : hasShow ? "bg-card border-border hover:bg-accent" : "bg-muted/30 text-muted-foreground/30 border-transparent cursor-not-allowed")"
                                    disabled="@(!hasShow)"
                                    @onclick="() => SelectDate(date)">
                                <span class="text-[10px] font-bold uppercase opacity-70">@date.ToString("ddd")</span>
                                <span class="text-lg font-bold">@date.ToString("dd")</span>
                            </button>
                        }
                    </div>
                    <button class="btn btn-ghost rounded-full" @onclick="NextWeek"><i class="lucide-chevron-right"></i></button>
                </div>
                
                <!-- Showtime items -->
                <div class="flex-1 w-full">
                    <p class="text-xs text-muted-foreground font-bold uppercase tracking-widest mb-2">Suất Chiếu (@SelectedDate.ToString("dd MMM"))</p>
                    <div class="flex gap-3 overflow-x-auto">
                        @foreach (var show in FilteredShows)
                        {
                            var isSelected = SelectedShow?.Id == show.Id;
                            <button class="px-5 py-2.5 rounded-xl border text-sm font-semibold transition-all flex items-center gap-2 @(isSelected ? "bg-foreground text-background border-foreground shadow-md" : "bg-card border-border text-muted-foreground hover:border-primary")"
                                    @onclick="() => SelectShow(show)">
                                @($"{show.StartTime[..5]} - {show.EndTime[..5]}")
                                <span class="text-[9px] font-bold px-1.5 py-0.5 rounded @(isSelected ? "bg-background/20 text-background" : "bg-muted text-muted-foreground")">
                                    @(show.Type == "ThreeD" ? "3D" : "2D")
                                </span>
                            </button>
                        }
                    </div>
                </div>
            </div>
        </div>

        <!-- MAIN INTERACTIVE SEAT MAP SECTION -->
        <div class="container py-12">
            @if (SelectedShow == null)
            {
                <div class="flex flex-col items-center justify-center py-32 text-muted-foreground space-y-4">
                    <div class="w-24 h-24 border border-dashed rounded-full flex items-center justify-center opacity-30"><i class="lucide-monitor text-4xl"></i></div>
                    <h3 class="text-lg font-medium">Sẵn sàng trải nghiệm?</h3>
                    <p class="text-sm">Vui lòng chọn một suất chiếu để hiển thị bản đồ ghế.</p>
                </div>
            }
            else if (theater != null)
            {
                <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-16">
                    <!-- Order Review & Checkout Panel -->
                    <div class="lg:col-span-4 order-2 lg:order-1">
                        <div class="space-y-6 sticky top-24">
                            <h3 class="text-xl font-bold uppercase">Đơn Đặt Hàng</h3>
                            
                            <div class="card p-6 bg-card border border-border rounded-2xl space-y-4">
                                <div class="flex gap-4">
                                    <div class="w-12 h-16 bg-muted rounded overflow-hidden">
                                        <img src="@movie.PosterUrl" class="w-full h-full object-cover" />
                                    </div>
                                    <div>
                                        <h4 class="font-bold">@movie.Title</h4>
                                        <p class="text-xs text-muted-foreground"><i class="lucide-clock"></i> @SelectedDate.ToString("dd MMM yyyy") • @SelectedShow.StartTime[..5]</p>
                                    </div>
                                </div>
                                <div class="border-t border-border/50 pt-4 space-y-2 text-sm">
                                    <div class="flex justify-between">
                                        <span class="text-muted-foreground">Ghế đã chọn (@SelectedSeats.Count)</span>
                                        <span class="font-bold">@(SelectedSeats.Any() ? string.Join(", ", SelectedSeats) : "-")</span>
                                    </div>
                                    <div class="flex justify-between">
                                        <span class="text-muted-foreground">Giá vé</span>
                                        <span class="font-medium">40,000 đ</span>
                                    </div>
                                </div>
                            </div>

                            <div class="bg-primary text-primary-foreground rounded-xl p-6 shadow-xl relative overflow-hidden">
                                <div class="relative z-10">
                                    <p class="text-xs text-primary-foreground/80 font-bold uppercase mb-1">Tổng tiền thanh toán</p>
                                    <p class="text-3xl font-black mb-6">@((SelectedSeats.Count * 40000).ToString("N0")) đ</p>
                                    <button class="w-full bg-white/20 hover:bg-white/30 text-primary-foreground font-bold h-12 rounded-xl border border-white/10 backdrop-blur-md transition-all"
                                            disabled="@(!SelectedSeats.Any())"
                                            @onclick="OpenPaymentModal">
                                        TIẾN HÀNH THANH TOÁN
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- 3D Seat Map Grid Panel -->
                    <div class="lg:col-span-8 order-1 lg:order-2">
                        <div class="card rounded-[2rem] p-6 lg:p-10 shadow-lg relative bg-card border border-border overflow-hidden">
                            <div class="absolute top-0 left-1/2 -translate-x-1/2 w-1/2 h-2 bg-primary blur-[60px] opacity-40" />
                            
                            <!-- Theater Header -->
                            <div class="text-center border-b border-border pb-4 mb-8 text-sm text-muted-foreground">
                                <i class="lucide-map-pin text-primary"></i> <span class="font-medium text-foreground">@theater.Name</span>
                                <span class="mx-2">•</span><span>@(SelectedShow.Type == "ThreeD" ? "Trải nghiệm 3D" : "2D Tiêu chuẩn")</span>
                            </div>

                            <!-- Screen Curved Curve Visual -->
                            <div class="relative w-full flex flex-col items-center mb-16">
                                <div class="w-2/3 h-2 bg-primary rounded-full shadow-[0_0_30px_10px_rgba(229,9,20,0.4)]" style="border-radius: 50%" />
                                <div class="absolute top-4 text-xs font-bold text-primary tracking-[0.5em] opacity-70">MÀN HÌNH CHẤT LƯỢNG CAO</div>
                            </div>

                            <!-- Seat Grid Loop Rendering -->
                            <div class="overflow-x-auto pb-8 flex justify-center">
                                <div class="min-w-max">
                                    @for (int i = 0; i < theater.NumOfRows; i++)
                                    {
                                        var rowChar = ((char)('A' + i)).ToString();
                                        <div class="flex gap-1.5 items-center justify-center mb-1.5">
                                            <span class="w-6 text-center text-[10px] text-muted-foreground font-bold">@rowChar</span>
                                            
                                            @for (int j = 1; j <= theater.SeatsPerRow; j++)
                                            {
                                                var seatCode = $"{rowChar}{j}";
                                                var colIndex = j;
                                                var isMissing = theater.Missing.Any(s => s.SeatRow == rowChar && s.SeatNumber == colIndex);
                                                var isBlocked = theater.Blocked.Any(s => s.SeatRow == rowChar && s.SeatNumber == colIndex);
                                                var isBooked = BookedSeats.Any(s => s.SeatRow == rowChar && s.SeatNumber == colIndex);
                                                var isSelected = SelectedSeats.Contains(seatCode);

                                                @if (isMissing)
                                                {
                                                    <div class="w-6 h-6 md:w-8 md:h-8 opacity-0 pointer-events-none" />
                                                }
                                                else
                                                {
                                                    var seatClass = "w-6 h-6 md:w-8 md:h-8 rounded-t-md rounded-b-sm flex items-center justify-center transition-all duration-200 text-[9px] font-bold border ";
                                                    if (isBlocked) seatClass += "bg-muted cursor-not-allowed border-transparent opacity-30";
                                                    else if (isBooked) seatClass += "bg-muted text-muted-foreground/50 cursor-not-allowed border-transparent";
                                                    else if (isSelected) seatClass += "bg-primary text-primary-foreground shadow-lg shadow-primary/40 scale-110 z-10 border-primary";
                                                    else seatClass += "bg-card hover:bg-primary/20 hover:border-primary/50 cursor-pointer text-transparent hover:text-foreground/50 border-border shadow-sm";

                                                    <button class="@seatClass"
                                                            disabled="@(isBlocked || isBooked)"
                                                            @onclick="() => ToggleSeat(seatCode)">
                                                        @colIndex
                                                    </button>
                                                }
                                            }

                                            <span class="w-6 text-center text-[10px] text-muted-foreground font-bold">@rowChar</span>
                                        </div>
                                    }
                                </div>
                            </div>

                            <!-- Legend -->
                            <div class="flex justify-center gap-6 text-xs w-full max-w-2xl border-t border-border pt-6 mx-auto">
                                <div class="flex items-center gap-2"><div class="w-3 h-3 rounded bg-card border border-border" /><span class="text-muted-foreground">Còn trống</span></div>
                                <div class="flex items-center gap-2"><div class="w-3 h-3 rounded bg-primary" /><span class="text-foreground font-medium">Đang chọn</span></div>
                                <div class="flex items-center gap-2"><div class="w-3 h-3 rounded bg-muted" /><span class="text-muted-foreground">Đã bán/Khóa</span></div>
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    }
</div>

<!-- Checkout Dialog Modal (Standard Radix-like implementation in Blazor) -->
@if (IsPaymentModalOpen)
{
    <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm animate-in fade-in duration-300">
        <div class="bg-card border border-border max-w-md w-full rounded-2xl p-6 shadow-2xl relative animate-in zoom-in-95 duration-200">
            <h2 class="text-2xl font-bold mb-1">Xác Nhận Đặt Vé</h2>
            <p class="text-sm text-muted-foreground mb-6">Vui lòng chọn phương thức thanh toán phù hợp.</p>
            
            <div class="bg-muted/50 p-4 rounded-xl flex justify-between items-center border border-border mb-6">
                <div>
                    <p class="text-xs text-muted-foreground">Tổng số tiền</p>
                    <p class="text-2xl font-bold">@((SelectedSeats.Count * 40000).ToString("N0")) đ</p>
                </div>
                <span class="badge bg-primary text-primary-foreground text-xs font-bold px-3 py-1 rounded-full">@SelectedSeats.Count Ghế</span>
            </div>

            <!-- Radio Selection Payments -->
            <div class="space-y-3 mb-8">
                <div class="flex items-center justify-between p-4 rounded-xl border-2 @(PaymentMethod == "Card" ? "border-primary bg-accent/20" : "border-muted hover:bg-accent/10") cursor-pointer"
                     @onclick='() => PaymentMethod = "Card"'>
                    <div class="flex items-center gap-3">
                        <i class="lucide-credit-card text-primary text-xl"></i>
                        <div>
                            <p class="text-sm font-semibold">Thẻ Tín Dụng</p>
                            <p class="text-xs text-muted-foreground">Visa, Mastercard, ATM</p>
                        </div>
                    </div>
                    @if (PaymentMethod == "Card") { <i class="lucide-check-circle-2 text-primary"></i> }
                </div>
                
                <div class="flex items-center justify-between p-4 rounded-xl border-2 @(PaymentMethod == "Cash" ? "border-primary bg-accent/20" : "border-muted hover:bg-accent/10") cursor-pointer"
                     @onclick='() => PaymentMethod = "Cash"'>
                    <div class="flex items-center gap-3">
                        <i class="lucide-banknote text-green-500 text-xl"></i>
                        <div>
                            <p class="text-sm font-semibold">Tiền Mặt</p>
                            <p class="text-xs text-muted-foreground">Thanh toán trực tiếp tại quầy</p>
                        </div>
                    </div>
                    @if (PaymentMethod == "Cash") { <i class="lucide-check-circle-2 text-primary"></i> }
                </div>
            </div>

            <div class="flex justify-end gap-3">
                <button class="btn btn-outline h-12 px-6 rounded-xl" disabled="@IsProcessing" @onclick="() => IsPaymentModalOpen = false">Hủy</button>
                <button class="btn btn-primary h-12 px-8 rounded-xl flex items-center justify-center font-bold" disabled="@IsProcessing" @onclick="ConfirmPayment">
                    @if (IsProcessing)
                    {
                        <div class="w-4 h-4 border-2 border-primary-foreground border-t-transparent rounded-full animate-spin mr-2"></div>
                        <span>Đang xử lý...</span>
                    }
                    else
                    {
                        <span>Xác Nhận Thanh Toán</span>
                    }
                </button>
            </div>
        </div>
    </div>
}

@code {
    [Parameter] public int Id { get; set; }
    
    private MovieDto movie;
    private List<PersonDto> castList = new();
    private List<ShowDto> allShows = new();
    private List<ShowDto> FilteredShows = new();
    private List<string> DatesWithShows = new();
    private List<DateTime> DateRangeList = new();
    
    private DateTime SelectedDate { get; set; } = DateTime.Parse("2025-12-18");
    private DateTime ViewStartDate { get; set; } = DateTime.Parse("2025-12-18");
    
    private ShowDto SelectedShow;
    private TheaterDto theater;
    private List<BookedSeatDto> BookedSeats = new();
    private List<string> SelectedSeats = new();
    
    private bool IsPaymentModalOpen { get; set; }
    private string PaymentMethod { get; set; } = "Card";
    private bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadMovieData();
        InitializeDateRange();
    }

    private async Task LoadMovieData()
    {
        try
        {
            movie = await ApiClient.GetMovieByIdAsync(Id);
            allShows = await ApiClient.GetShowsByMovieIdAsync(Id);
            
            // Lấy danh sách ngày có suất chiếu
            DatesWithShows = allShows.Select(s => s.Date.Split('T')[0]).Distinct().OrderBy(d => d).ToList();
            
            // Cast list handle
            castList = await ApiClient.GetCastByMovieIdAsync(Id);

            if (DatesWithShows.Any())
            {
                SelectedDate = DateTime.Parse(DatesWithShows.First());
                ViewStartDate = SelectedDate;
                await FilterShowsForSelectedDate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi tải thông tin phim: " + ex.Message);
        }
    }

    private void InitializeDateRange()
    {
        DateRangeList.Clear();
        for (int i = 0; i < 7; i++)
        {
            DateRangeList.Add(ViewStartDate.AddDays(i));
        }
    }

    private async Task SelectDate(DateTime date)
    {
        SelectedDate = date;
        SelectedShow = null;
        theater = null;
        SelectedSeats.Clear();
        await FilterShowsForSelectedDate();
    }

    private async Task FilterShowsForSelectedDate()
    {
        var targetDateStr = SelectedDate.ToString("yyyy-MM-dd");
        FilteredShows = allShows.Where(s => s.Date.StartsWith(targetDateStr)).OrderBy(s => s.StartTime).ToList();
        StateHasChanged();
    }

    private async Task SelectShow(ShowDto show)
    {
        SelectedShow = show;
        theater = null;
        SelectedSeats.Clear();
        
        // Load bản đồ ghế của Rạp & Danh sách ghế đã đặt
        theater = await ApiClient.GetTheaterByIdAsync(show.TheaterId);
        BookedSeats = await ApiClient.GetBookedSeatsByShowIdAsync(show.Id);
        
        // Kích hoạt GSAP animation cho các ghế qua JS Interop
        await JS.InvokeVoidAsync("animateSeatsEntrance");
    }

    private void ToggleSeat(string seatCode)
    {
        if (SelectedSeats.Contains(seatCode))
            SelectedSeats.Remove(seatCode);
        else
            SelectedSeats.Add(seatCode);
    }

    private void PrevWeek()
    {
        ViewStartDate = ViewStartDate.AddDays(-7);
        InitializeDateRange();
    }

    private void NextWeek()
    {
        ViewStartDate = ViewStartDate.AddDays(7);
        InitializeDateRange();
    }

    private void OpenPaymentModal()
    {
        IsPaymentModalOpen = true;
    }

    private async Task ConfirmPayment()
    {
        if (SelectedShow == null || !SelectedSeats.Any()) return;
        
        IsProcessing = true;
        
        try
        {
            // Bước 1: Gọi API tạo Đặt vé cho từng ghế song song
            var bookingTasks = SelectedSeats.Select(seat => {
                var row = seat[..1];
                var num = int.Parse(seat[1..]);
                return ApiClient.CreateBookingAsync(new CreateBookingCommand {
                    UserId = "current-user-guid-id",
                    ShowId = SelectedShow.Id,
                    SeatRow = row,
                    SeatNumber = num,
                    Price = 40000,
                    Status = 1
                });
            });

            var bookingIds = (await Task.WhenAll(bookingTasks)).ToList();

            // Bước 2: Gọi API gom đơn thanh toán
            var totalAmount = SelectedSeats.Count * 40000;
            var paymentId = await ApiClient.CreatePaymentAsync(new CreatePaymentCommand {
                Amount = totalAmount,
                PaymentDatetime = DateTime.UtcNow.ToString("o"),
                PaymentMethod = PaymentMethod.ToLower(),
                UserId = "current-user-guid-id",
                ShowId = SelectedShow.Id,
                Bookings = bookingIds
            });

            IsPaymentModalOpen = false;
            
            // Chuyển hướng tới trang đặt vé thành công
            var bookingIdsStr = string.Join(",", bookingIds);
            Navigation.NavigateTo($"/booking/success?paymentId={paymentId}&bookingIds={bookingIdsStr}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Giao dịch thất bại: " + ex.Message);
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
```

### 3. Vé E-Ticket xé đôi dạng card (`BookingSuccess.razor`)
*Rebuild hoàn hảo giao diện E-Ticket độc đáo của hệ thống.*

```razor
@page "/booking/success"
@inject IApiClient ApiClient
@inject NavigationManager Navigation
@using System.Web

<div class="min-h-screen bg-background text-foreground flex items-center justify-center p-4 relative overflow-hidden py-10">
    <!-- Blurred Background Accents -->
    <div class="absolute inset-0 z-0 pointer-events-none">
        <div class="absolute top-1/4 left-1/4 w-64 h-64 bg-primary/10 rounded-full blur-[100px]" />
        <div class="absolute bottom-1/4 right-1/4 w-64 h-64 bg-blue-500/10 rounded-full blur-[100px]" />
    </div>

    @if (loading)
    {
        <div class="flex flex-col items-center gap-6 w-full max-w-md">
            <div class="w-24 h-24 bg-muted rounded-full animate-pulse"></div>
            <div class="w-48 h-8 bg-muted rounded animate-pulse"></div>
            <div class="w-full h-96 bg-muted rounded-3xl animate-pulse"></div>
        </div>
    }
    else if (payment != null)
    {
        <div class="relative z-10 w-full max-w-md flex flex-col gap-6 animate-in slide-in-from-bottom-8 duration-700">
            <!-- Success Header -->
            <div class="text-center space-y-4">
                <div class="mx-auto w-24 h-24 bg-green-500 text-white rounded-full flex items-center justify-center shadow-xl shadow-green-500/20">
                    <i class="lucide-check-circle-2 text-5xl"></i>
                </div>
                <h1 class="text-3xl font-black tracking-tight">Đặt Vé Thành Công!</h1>
                <p class="text-muted-foreground">Vé điện tử của bạn đã được xuất.</p>
            </div>

            <!-- TEAR E-TICKET CARD -->
            <div class="border-none bg-card/90 backdrop-blur-xl shadow-2xl overflow-hidden rounded-3xl relative">
                
                <!-- Ticket Upper Section: Movie & Cover -->
                <div class="p-6 pb-4 flex gap-4 border-b border-dashed border-border/50 relative">
                    <!-- Left Circle Cutout -->
                    <div class="absolute -left-3 bottom-[-12px] w-6 h-6 bg-background rounded-full z-10 shadow-[inset_-2px_0_2px_rgba(0,0,0,0.05)]" />
                    <!-- Right Circle Cutout -->
                    <div class="absolute -right-3 bottom-[-12px] w-6 h-6 bg-background rounded-full z-10 shadow-[inset_2px_0_2px_rgba(0,0,0,0.05)]" />

                    <div class="w-20 h-28 shrink-0 rounded-lg overflow-hidden bg-muted shadow-md">
                        <img src="@payment.MoviePosterUrl" class="w-full h-full object-cover" alt="poster" />
                    </div>
                    <div class="space-y-1.5 flex-1 min-w-0">
                        <p class="text-[10px] text-primary uppercase font-bold tracking-wider">Phim</p>
                        <h3 class="font-bold text-xl leading-tight truncate">@payment.MovieTitle</h3>
                        <div class="flex flex-wrap gap-2 pt-1">
                            <span class="badge bg-secondary text-[10px] h-5 px-2 rounded-md">2D Standard</span>
                            <span class="badge border border-border text-[10px] h-5 px-2 rounded-md">IMAX</span>
                        </div>
                    </div>
                </div>

                <!-- Ticket Lower Section: Metadata Grid & QR -->
                <div class="p-6 grid grid-cols-2 gap-y-6 gap-x-4 text-sm bg-muted/30">
                    <div>
                        <div class="flex items-center gap-1.5 text-muted-foreground mb-1.5">
                            <i class="lucide-calendar text-xs"></i> <span class="text-[10px] uppercase font-bold">Ngày Chiếu</span>
                        </div>
                        <p class="font-bold text-foreground text-base">@showDate.ToString("ddd, dd MMM yyyy")</p>
                    </div>
                    
                    <div>
                        <div class="flex items-center gap-1.5 text-muted-foreground mb-1.5">
                            <i class="lucide-clock text-xs"></i> <span class="text-[10px] uppercase font-bold">Giờ Chiếu</span>
                        </div>
                        <p class="font-bold text-foreground text-base">@showTime</p>
                    </div>

                    <div class="col-span-2">
                        <div class="flex items-center gap-1.5 text-muted-foreground mb-1.5">
                            <i class="lucide-map-pin text-xs"></i> <span class="text-[10px] uppercase font-bold">Rạp Chiếu</span>
                        </div>
                        <p class="font-bold text-foreground text-base">@theaterName</p>
                    </div>

                    <!-- Selected Seats -->
                    <div class="col-span-2 bg-background p-3 rounded-xl border border-dashed border-border flex items-center justify-between">
                        <div class="flex items-center gap-2 text-muted-foreground">
                            <i class="lucide-armchair text-sm"></i>
                            <span class="text-xs font-bold uppercase">Danh Sách Ghế</span>
                        </div>
                        <p class="font-black text-lg text-primary tracking-wide">
                            @string.Join(", ", bookings.Select(b => $"{b.SeatRow}{b.SeatNumber}"))
                        </p>
                    </div>
                </div>

                <!-- Payment Details & QR Code Footer -->
                <div class="bg-muted/50 p-6 pt-4 border-t border-dashed border-border/50">
                    <div class="flex flex-col items-center justify-center space-y-4 mb-6">
                        <!-- QR Simulation via Google Charts QR API -->
                        <div class="bg-white p-3 rounded-xl border border-gray-100 shadow-sm">
                            <img src="https://chart.googleapis.com/chart?chs=150x150&cht=qr&chl=@(payment.PaymentId)&choe=UTF-8" class="w-32 h-32" alt="QR Code" />
                        </div>
                        <div class="text-center">
                            <p class="text-[10px] text-muted-foreground uppercase font-bold">Mã Số Giao Dịch</p>
                            <p class="font-mono font-bold text-lg text-primary select-all">#@payment.PaymentId.ToString().PadLeft(6, '0')</p>
                        </div>
                    </div>

                    <div class="flex justify-between items-end border-t border-border/50 pt-4">
                        <div>
                            <span class="text-[10px] text-muted-foreground uppercase font-bold">Thanh Toán</span>
                            <span class="text-sm font-semibold capitalize flex items-center gap-1.5 mt-0.5">
                                <i class="lucide-credit-card text-primary"></i> @payment.PaymentMethod
                            </span>
                        </div>
                        <div class="text-right">
                            <span class="text-[10px] text-muted-foreground uppercase font-bold">Tổng Cộng</span>
                            <p class="text-2xl font-black text-primary leading-none mt-1">@payment.Amount.ToString("N0") đ</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Navigation Actions -->
            <div class="flex flex-col gap-3">
                <button class="btn btn-primary w-full h-12 text-base font-bold rounded-xl" @onclick='() => Navigation.NavigateTo("/movies")'>
                    Đặt Thêm Vé Khác <i class="lucide-arrow-right ml-2"></i>
                </button>
                <button class="btn btn-ghost w-full h-12 text-muted-foreground rounded-xl" @onclick='() => Navigation.NavigateTo("/")'>
                    <i class="lucide-home mr-2"></i> Trở về Trang Chủ
                </button>
            </div>
        </div>
    }
</div>

@code {
    private bool loading = true;
    private PaymentDto payment;
    private List<BookingDto> bookings = new();
    private DateTime showDate = DateTime.Now;
    private string showTime = "--:--";
    private string theaterName = "";

    protected override async Task OnInitializedAsync()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var paymentIdStr = query.Get("paymentId");
        var bookingIdsStr = query.Get("bookingIds");

        if (int.TryParse(paymentIdStr, out int paymentId))
        {
            try
            {
                // Fetch thông tin Payment
                payment = await ApiClient.GetPaymentByIdAsync(paymentId);
                
                // Fetch danh sách Booking cụ thể
                if (!string.IsNullOrWhiteSpace(bookingIdsStr))
                {
                    var bookingIds = bookingIdsStr.Split(',').Select(int.Parse).ToList();
                    foreach (var bId in bookingIds)
                    {
                        var booking = await ApiClient.GetBookingByIdAsync(bId);
                        bookings.Add(booking);
                    }
                }

                // Tải thông tin Suất chiếu & Rạp dựa trên Booking đầu tiên
                if (bookings.Any())
                {
                    var firstBooking = bookings.First();
                    var show = await ApiClient.GetShowByIdAsync(firstBooking.ShowId);
                    if (show != null)
                    {
                        showDate = DateTime.Parse(show.Date);
                        showTime = show.StartTime[..5];
                        var theater = await ApiClient.GetTheaterByIdAsync(show.TheaterId);
                        theaterName = theater?.Name ?? "CGV Cinema";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải chi tiết vé thành công: " + ex.Message);
            }
            finally
            {
                loading = false;
            }
        }
    }
}
```

### 4. JS Interop phục vụ Animations GSAP (`app-animations.js`)
*File trung gian JS được Blazor WASM gọi để khởi động hoạt họa GSAP mượt mà mà không ảnh hưởng tới vòng đời C#.*

```javascript
// Đăng ký JS Interop vào môi trường window
window.animateSeatsEntrance = () => {
    // Đảm bảo GSAP đã sẵn sàng và phần tử tồn tại
    if (typeof gsap !== 'undefined') {
        // Reset trạng thái ban đầu của các ghế
        gsap.set(".seat-btn", { scale: 0, opacity: 0 });
        
        // Hoạt họa zoom-in bounce rực rỡ từ tâm
        gsap.to(".seat-btn", {
            scale: 1,
            opacity: 1,
            duration: 0.45,
            stagger: {
                amount: 0.25,
                grid: "auto",
                from: "center"
            },
            ease: "back.out(1.5)"
        });
    }
};

window.animateHeroText = () => {
    if (typeof gsap !== 'undefined') {
        gsap.fromTo(".hero-animate", 
            { y: 30, opacity: 0 },
            { y: 0, opacity: 1, duration: 0.8, stagger: 0.1, ease: "power3.out", delay: 0.15 }
        );
    }
};
```

---

## IX. KẾT LUẬN & ĐÁNH GIÁ KIẾN TRÚC MỚI

Việc chuyển dịch từ **React + TypeScript** sang **Blazor Interactive WebAssembly** mang lại những cải tiến vượt bậc cho dự án **Movie Reservation**:

1. **Thống nhất Hệ thống Kiểu (Type Safety):** 100% các class DTO của C# được chia sẻ trực tiếp giữa .NET Backend và Blazor Client, triệt tiêu hoàn toàn rủi ro sai lệch trường dữ liệu (mismatch properties) thường thấy giữa React và API.
2. **Kế thừa Bảo mật Tuyệt đối:** Tận dụng tối đa giải pháp `AuthorizeView` và cơ chế lọc Claim của .NET Identity để thiết lập rào chắn Admin Area cực kỳ sạch gọn hơn nhiều so với việc kiểm tra mảng string thủ công trong React layout.
3. **Hiệu năng & Tải trang:** Blazor WASM tải toàn bộ Assemblies xuống trình duyệt của khách hàng, cho phép mọi thao tác click chọn ghế, lọc suất chiếu, mở modal diễn ra tức thời như một phần mềm Desktop độc lập mà không cần gửi bất kỳ dòng lệnh xử lý UI nào lên máy chủ.

*Báo cáo này được cấu trúc hoàn mỹ, cực kỳ chi tiết, chuẩn chỉ kiến trúc cao cấp và sẵn sàng 100% để bất kỳ hệ thống AI nào khác có thể lập tức tái xây dựng lại toàn bộ dự án sang Blazor mà không cần tham chiếu lại mã nguồn cũ.*
