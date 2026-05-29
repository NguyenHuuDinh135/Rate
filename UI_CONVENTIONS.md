# Quy chuẩn Thiết kế & Lập trình Giao diện (UI/UX Conventions) - Dự án RATE

Tài liệu này định nghĩa hệ thống thiết kế (Design System), quy chuẩn giao diện (UI) và cấu trúc component (UX) hiện tại của dự án **RATE (Cinema Booking)**. Mọi trang giao diện mới hoặc các cải tiến về sau **bắt buộc** phải tuân thủ đúng cấu trúc, màu sắc và quy chuẩn lập trình được mô tả dưới đây để đảm bảo tính đồng bộ toàn hệ thống.

---

## 🎨 1. Hệ thống Thiết kế & Màu sắc (Design Tokens)

Dự án sử dụng **Tailwind CSS** làm framework chính cho việc dàn trang và tạo kiểu dáng, kết hợp với các Web Components của **Microsoft Fluent UI**.

### 🔴 Bảng màu chính (Harmonious Color Palette)
*   **Màu chủ đạo & Nhấn (Primary/Accent)**: Đỏ rạp phim sang trọng `#E50914` (Netflix Red).
    *   Tên class Tailwind: `bg-accent`, `text-accent`, `border-accent`.
    *   Hiệu ứng hover tương ứng: `bg-accent-hover` (`#B20710`).
*   **Màu nền chính (Backgrounds)**:
    *   *Light Mode*: `bg-slate-50` hoặc `bg-slate-100`.
    *   *Dark Mode*: `bg-slate-950` hoặc `bg-[#131313]` (tối sâu).
*   **Màu chữ (Typography Colors)**:
    *   *Chữ chính*: `text-slate-950` (Light) / `text-white` (Dark).
    *   *Chữ phụ / Chú thích*: `text-slate-500` / `text-slate-400` / `text-[#a0a0a0]`.
*   **Màu trạng thái / Đánh giá (Status Colors)**:
    *   *Đánh giá/Sao vàng*: `#f59e0b` (Amber/Gold).
    *   *Thành công (Success)*: Màu xanh lục (`success`).
    *   *Lỗi / Cảnh báo*: Màu đỏ đậm (`error` / `danger`).

### ✍️ Phông chữ & Kiểu chữ (Typography)
*   **Font Family**: Thứ tự ưu tiên là `Inter`, `Manrope` hoặc phông chữ `sans-serif` tương đương, mang lại cảm giác hiện đại và dễ đọc.
*   **Kiểu chữ Tiêu đề (Headings)**:
    *   Luôn in đậm cực độ: `font-black`.
    *   Khoảng cách chữ hẹp: `tracking-tight` hoặc `tracking-tighter`.
    *   Khoảng cách dòng ngắn: `leading-tight` hoặc `leading-none`.
    *   Hầu hết tiêu đề lớn được viết hoa (`uppercase`).

### 📐 Hằng số bố cục (Layout Constants)
*   **Chiều rộng tối đa (Max Width)**: Giới hạn tối đa của layout container là `max-w-7xl` hoặc `max-w-[1440px]`.
*   **Đệm lề ngang (Horizontal Padding)**: Trên màn hình máy tính (Desktop) luôn là khoảng `5%` hoặc `px-4 sm:px-6 lg:px-8`.
*   **Bo góc (Border Radius)**: Bo góc cực mềm mại theo phong cách bo rạp phim: `rounded-[2rem]`, `rounded-[2.5rem]` hoặc `rounded-cinema` (`24px`).

---

## 🧩 2. Cấu trúc Component & Quy chuẩn HTML/Blazor

### 🏷 Tiêu đề trang (`<PageTitle>`)
Mỗi trang `.razor` cần khai báo tiêu đề trang đầu tiên:
```razor
<PageTitle>Tên Trang | RATE</PageTitle>
```

### 💫 Hiệu ứng chuyển cảnh & Hoạt họa (Animations)
Các trang khi được render nên có hiệu ứng chuyển đổi mượt mà bằng các class animation của Tailwind:
*   **Fade-in cơ bản**: `animate-in fade-in duration-500`
*   **Slide-in từ dưới**: `animate-in fade-in slide-in-from-bottom-8 duration-700`

### 📽 Tải ảnh & Placeholder thông minh (Fallback Placeholders)
Để tránh hiện tượng vỡ khung hình hoặc ảnh lỗi, mọi thẻ `<img>` hiển thị poster/banner phim bắt buộc phải có ảnh nền placeholder dự phòng:
```html
<div class="relative aspect-[2/3] overflow-hidden rounded-[2rem] bg-gradient-to-b from-slate-900 to-slate-950">
    <!-- Nền dự phòng khi ảnh chưa tải xong hoặc bị lỗi -->
    <div class="absolute inset-0 flex flex-col items-center justify-center p-6 text-center select-none bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950">
        <FluentIcon Value="@(new Size24.MoviesAndTv())" Color="Color.Accent" class="w-8 h-8 mb-2" />
        <span class="text-xs font-black text-white/95 uppercase">@MovieTitle</span>
    </div>

    <!-- Ảnh poster động -->
    <img src="@PosterUrl" 
         onerror="this.style.opacity='0';"
         class="absolute inset-0 z-10 w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" 
         alt="@MovieTitle" />
</div>
```

### 🔴 Quy chuẩn Nút hành động cao cấp (Premium Action Buttons)
Các nút chính trong hệ thống (đặt vé, thanh toán) cần thiết kế bắt mắt, có đổ bóng rực rỡ và hiệu ứng phản hồi nhấn:
```html
<button class="px-8 py-4 bg-accent hover:bg-accent-hover text-white text-xs font-black uppercase tracking-widest rounded-2xl shadow-xl shadow-accent/25 hover:shadow-accent/45 hover:scale-[1.02] transition-all duration-300 active:scale-95 cursor-pointer flex items-center justify-center gap-2">
    <FluentIcon Value="@(new Size20.TicketDiagonal())" Color="Color.Fill" class="w-5 h-5 text-white" />
    <span>MUA VÉ NGAY</span>
</button>
```

---

## ⚙️ 3. Quy chuẩn Lập trình logic (Programming Conventions)

### 🔄 Quản lý Trạng thái với Fluxor (CQRS Pattern)
Đối với các dữ liệu dùng chung (phim, rạp, tài khoản), sử dụng thư viện **Fluxor** để quản lý luồng dữ liệu (State/Action/Reducer):
*   Khởi tạo tiêm phụ thuộc (Dependency Injection):
    ```csharp
    @inject IState<MovieState> MovieState
    @inject IDispatcher Dispatcher
    ```
*   Tải dữ liệu trong `OnInitialized()`:
    ```csharp
    protected override void OnInitialized()
    {
        if (MovieState.Value.Movies.Count == 0)
        {
            Dispatcher.Dispatch(new LoadMoviesAction());
        }
    }
    ```

### ⏳ Trạng thái Đang tải (Loading State)
Luôn hiển thị màn hình chờ đẹp mắt khi dữ liệu chưa tải xong:
```razor
@if (MovieState.Value.IsLoading && MovieState.Value.SelectedMovie == null)
{
    <div class="flex flex-col items-center justify-center min-h-[50vh] gap-4">
        <div class="w-12 h-12 rounded-full border-4 border-accent border-t-transparent animate-spin"></div>
        <p class="text-xs font-black uppercase tracking-widest text-slate-500 animate-pulse">Đang tải dữ liệu...</p>
    </div>
}
```

### ⚠️ Trạng thái Lỗi / Không tìm thấy (Error/Empty State)
Khi truy vấn API thất bại hoặc dữ liệu trống, cần có giao diện thông báo trực quan:
```razor
else if (payment == null)
{
    <div class="flex flex-col items-center justify-center p-12 bg-white rounded-[2rem] shadow-xl border border-slate-100 gap-4 text-center">
        <FluentIcon Value="@(new Size48.Warning())" Color="Color.Error" />
        <h2 class="text-xl font-black text-slate-800">Không tìm thấy thông tin!</h2>
        <p class="text-slate-400 text-sm">Giao dịch này không tồn tại hoặc đã bị hủy.</p>
        <FluentButton Appearance="Appearance.Accent" OnClick="@GoToHome">Trở lại trang chủ</FluentButton>
    </div>
}
```

---

## 🌟 4. Yêu cầu cho các thiết kế về sau

Khi thêm bất kỳ trang mới nào (ví dụ: Trang quản trị, trang sự kiện, trang chi tiết rạp chiếu):
1.  **Duy trì tính nhất quán**: Sử dụng chung các biến màu Tailwind và cấu trúc thẻ HTML như đã quy định.
2.  **Khử trùng lặp code (Don't Repeat Yourself - DRY)**: Chia nhỏ các thành phần giao diện lặp lại thành các Component dùng chung nằm trong thư mục `src/WebUI/Shared/Components`.
3.  **Tương thích di động (Mobile First/Responsive)**: Mọi giao diện bắt buộc phải hoạt động mượt mà trên cả Desktop, Tablet và Mobile bằng cách sử dụng các prefix responsive của Tailwind (`sm:`, `md:`, `lg:`, `xl:`).
4.  **Tương tác tinh tế (Micro-interactions)**: Thêm các hiệu ứng `transition-all duration-300`, `hover:scale-105` cho thẻ phim, nút bấm hoặc các mục danh sách để tăng độ mượt mà khi sử dụng.
