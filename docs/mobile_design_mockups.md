# Thư Viện Thiết Kế Giao Diện Khách Hàng Mobile .NET MAUI Blazor Hybrid - RATE

Tài liệu này chứa **trọn bộ 14 bản thiết kế UI Mockup Premium (Dark Mode)** chất lượng cao, bao phủ toàn vẹn 100% luồng trải nghiệm khách hàng di động (Customer Mobile App) mà không có bất kỳ lỗ hổng giao diện nào. Toàn bộ thiết kế này tương ứng 1-1 với dự án `WebFrontend.Shared`.

---

## 🎨 Trực Quan Hóa Giao Diện (UI Mockups Carousel - 14 Màn Hình Khách Hàng)

````carousel
![1. Màn hình Chào mừng (Onboarding Screen) giới thiệu các tính năng độc quyền của app RATE với neon logo.](./images/onboarding.png)
<!-- slide -->
![2. Màn hình Đăng nhập (Login Screen) hỗ trợ nhập email/password, FaceID và đăng nhập bằng mạng xã hội.](./images/login.png)
<!-- slide -->
![3. Màn hình Đăng ký (Register Screen) thiết kế form đăng ký tài khoản mới tối giản và hiện đại.](./images/register.png)
<!-- slide -->
![4. Màn hình Xác thực OTP (Verify OTP Screen) ô nhập mã đứt quãng và bàn phím số tích hợp mượt mà.](./images/otp.png)
<!-- slide -->
![5. Màn hình Trang chủ (Home Screen) chứa thanh tìm kiếm, Banner nổi bật, các danh mục Phim đang chiếu & Sắp chiếu.](./images/home_screen.png)
<!-- slide -->
![6. Màn hình Chi tiết phim (Movie Details) hiển thị thông tin phim Dune, trailer, cùng tab đánh giá/review người dùng.](./images/movie_details.png)
<!-- slide -->
![7. Màn hình Chọn Suất chiếu & Rạp (Showtime Selection) hiển thị thanh tiến trình, Date Picker ngang và danh sách suất chiếu.](./images/showtime_selection.png)
<!-- slide -->
![8. Màn hình Sơ đồ chọn ghế (Seat Selection) trực quan hóa sơ đồ ghế (VIP, Thường, Sweetbox) và nút giữ ghế đếm ngược.](./images/seat_selection.png)
<!-- slide -->
![9. Màn hình Chọn Bắp nước (Concessions Up-sell) hiển thị danh mục các gói Combo bỏng ngô và đồ uống kèm nút Skip nhanh.](./images/concessions.png)
<!-- slide -->
![10. Màn hình Thanh toán (Checkout) hiển thị hóa đơn dạng xé rách nghệ thuật, nhập mã giảm giá và cổng ví điện tử.](./images/checkout.png)
<!-- slide -->
![11. Màn hình Đặt vé thành công (Booking Success) hiển thị pháo hoa, mã số đặt chỗ, và nút xem vé nhanh.](./images/booking_success.png)
<!-- slide -->
![12. Màn hình Vé của tôi (My Tickets) hiển thị mã QR Code điện tử rõ nét để soát vé ngoại tuyến cực mượt.](./images/my_tickets.png)
<!-- slide -->
![13. Màn hình Hồ sơ & Thẻ thành viên (Profile & Loyalty) thẻ Gold Member điện tử kèm mã vạch để quét tại quầy rạp.](./images/profile.png)
<!-- slide -->
![14. Hộp thoại Đánh giá & Tag cảm xúc (Write Review Dialog) chấm sao và tag nhanh tâm trạng phim một cách trực quan.](./images/write_review_dialog.png)
````

---

## 🛠 Bản Đồ Ánh Xạ Giữa Giao Diện Mobile & File Mã Nguồn (Blazor Shared)

Để lập trình các giao diện trên, bạn chỉ cần chỉnh sửa/tạo mới các file Razor component trong thư mục `src/WebFrontend.Shared/Pages`.

| Màn Hình Trên Mobile | File Blazor Tương Ứng trong `WebFrontend.Shared` | Công Nghệ / Thư Viện Sử Dụng |
| :--- | :--- | :--- |
| **1. Màn hình Chào mừng (Onboarding)** | `WelcomeOnboarding.razor` *(NEW)* | TailwindCSS, `FluentButton` |
| **2. Màn hình Đăng nhập (Login)** | [Login.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Login.razor) | FaceID Integration UI, Social Auths |
| **3. Màn hình Đăng ký (Register)** | [Register.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Register.razor) | Modern Form, Validation Input |
| **4. Màn hình Xác thực OTP** | [VerifyOtp.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/VerifyOtp.razor) | Dynamic Keypad, Autoincrement Focus |
| **5. Trang Chủ (Home)** | [Home.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Home.razor) | TailwindCSS, `FluentButton`, `MovieState` (Fluxor) |
| **6. Chi tiết Phim** | [MovieDetails.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/MovieDetails.razor) | TailwindCSS, `FluentTabs`, `FluentIcon` |
| **7. Chọn Suất chiếu & Rạp** | `ShowtimeSelection.razor` *(NEW)* | Horizontal Date Picker, Booking Stepper |
| **8. Chọn Ghế (Seat Selection)** | `SeatSelection.razor` *(NEW)* nhúng [SeatMap.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Components/SeatMap.razor) | SVG/HTML Seat Matrix, Pinch-to-zoom |
| **9. Chọn Bắp nước (Concessions)** | `ConcessionsSelection.razor` *(NEW)* | Up-sell Items, Popcorn / Drinks List |
| **10. Thanh toán & Hóa đơn (Checkout)** | `Checkout.razor` *(NEW)* nhúng `PaymentDialog.razor` | Apple Pay, MoMo Integration UI |
| **11. Đặt vé thành công (Success)** | [BookingSuccess.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/BookingSuccess.razor) | Confetti Animation, Add to Calendar |
| **12. Vé Của Tôi & QR Code** | [MyTickets.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/MyTickets.razor) | Refit API Client, QR Code generator |
| **13. Hồ sơ & Thẻ thành viên (Profile)** | [Profile.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Profile.razor) | Barcode Generator, Settings Items |
| **14. Đăng ký & Viết Đánh giá** | `WriteReviewDialog.razor` *(NEW)* | Star Rating, Custom Emotion Tags |

---

## 🚀 Các Bước Cần Làm Tiếp Theo trong Codebase
Khi bạn bắt đầu lập trình:
1. **Setup Shell MAUI**: Đăng ký `BlazorWebView` trong `src/MobileApp` để load trang `Routes.razor`.
2. **Khởi tạo 5 Page mới**: Tạo các file `.razor` được đánh dấu `*(NEW)*` trong thư mục `WebFrontend.Shared/Pages/`.
3. **Thêm API Endpoint**: Tạo endpoint backend tương ứng (Ví dụ: GetShowtimes, CreateBooking, AddReview) trong `src/Web/Endpoints/`.
