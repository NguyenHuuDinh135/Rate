# Kế Hoạch Triển Khai & Danh Sách Toàn Bộ Màn Hình Cho Ứng Dụng Mobile .NET MAUI Blazor Hybrid - RATE (Cập nhật tiêu chuẩn UI/UX CGV, AMC, Fandango)

Bản kế hoạch dưới đây được xây dựng dựa trên kết quả nghiên cứu luồng trải nghiệm người dùng (UX Flows) của các chuỗi rạp lớn toàn cầu như **CGV (Hàn Quốc/Việt Nam), AMC (Mỹ), Fandango (Mỹ)** và các xu hướng thiết kế giao diện di động hiện đại từ **Dribbble & Behance**.

Để nâng tầm ứng dụng **RATE** đạt chuẩn thương mại premium, chúng tôi cập nhật danh sách màn hình và tích hợp các tính năng tối ưu hóa chuyển đổi (Conversion Rate Optimization).

---

## User Review Required

> [!IMPORTANT]
> **Các tính năng UI/UX cần thống nhất thiết kế:**
> 1. **Màn hình Bắp Nước (F&B / Concessions Up-sell)**: Nghiên cứu cho thấy bán bắp nước chiếm tới 30-40% lợi nhuận của rạp chiếu phim. Chúng tôi đã bổ sung thêm màn hình này vào quy trình đặt vé. Bạn có muốn cho phép bỏ qua nhanh (Skip) để rút ngắn thời gian đặt vé nếu khách không có nhu cầu không?
> 2. **Chế độ lưu vé Offline (Local Storage / Secure Wallet)**: Sóng điện thoại tại sảnh rạp chiếu phim (thường ở tầng hầm hoặc trung tâm thương mại) thường rất yếu. Chúng tôi đề xuất cơ chế tự động lưu thông tin vé (bao gồm QR Code) vào bộ nhớ cục bộ (Local Storage/Secure Storage) hoặc tích hợp nút xuất ra Apple Wallet / Google Wallet để soát vé ngoại tuyến.

---

## Open Questions

> [!WARNING]
> * **Câu hỏi làm rõ:**
>   - Bạn muốn áp dụng **định vị GPS tự động** của thiết bị để tìm và sắp xếp rạp gần nhất lên trên cùng, hay người dùng sẽ chọn khu vực (tỉnh/thành phố) thủ công thông qua một hộp thoại (Dropdown)?
>   - Hệ thống đánh giá phim có nên chia nhỏ thành **Đánh giá theo tiêu chí** (Kịch bản, Kỹ xảo, Diễn xuất, Nhạc phim) hay chỉ là chấm sao tổng quan kèm tag nhanh (ví dụ: #KịchTính, #HàiHước, #KỹXảoĐỉnh) giống cách CGV Golden Class đang làm?

---

## Proposed Changes

Chúng ta sẽ mở rộng quy trình đặt vé từ 23 lên **25 màn hình và file cấu hình** (bổ sung màn hình Bắp Nước F&B và màn hình Tìm kiếm/Bản đồ rạp gần nhất).

---

### 1. Phân Hệ Khách Hàng (Core Customer Mobile Flow)

#### [NEW] [WelcomeOnboarding.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/WelcomeOnboarding.razor)
* **Chức năng**: Màn hình giới thiệu ban đầu (Splash & Onboarding).
* **Tiêu chuẩn UX**: Sử dụng hiệu ứng chuyển động mượt mà (Lottie animations) giới thiệu các tính năng độc quyền: Đặt vé 30 giây, Review phim không spoil, Tích điểm thành viên.

#### [MODIFY] [Login.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Login.razor)
* **Chức năng**: Đăng nhập tài khoản.
* **Tiêu chuẩn UX**: Đăng nhập một chạm (Biometrics/FaceID) hoặc Social Logins (Google, Apple ID) để giảm thiểu tỷ lệ thoát ứng dụng (Bounce rate).

#### [MODIFY] [Register.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Register.razor)
* **Chức năng**: Đăng ký tài khoản người dùng mới.

#### [MODIFY] [VerifyOtp.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/VerifyOtp.razor)
* **Chức năng**: Xác thực OTP.
* **Tiêu chuẩn UX**: Thiết kế ô nhập mã dạng đứt quãng (Code Input boxes) tự động nhảy focus, đếm ngược thời gian gửi lại thông minh.

#### [MODIFY] [Home.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Home.razor)
* **Chức năng**: Trang chủ khám phá điện ảnh.
* **Tiêu chuẩn UX (CGV/Fandango style)**: 
  * Carousel banner tràn viền (Edge-to-Edge) tự động phát trailer ngắn dạng im lặng.
  * Tab kép chuyển đổi nhanh giữa: "Phim đang chiếu (Now Showing)" và "Phim hot sắp chiếu (Coming Soon)".
  * Hiển thị điểm số đánh giá ngay góc poster phim để kích thích người dùng click vào xem.

#### [MODIFY] [Explore.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Explore.razor)
* **Chức năng**: Tìm kiếm & Lọc phim.
* **Tiêu chuẩn UX**: Thanh tìm kiếm thông minh hỗ trợ tìm theo Tên phim, Thể loại, Rạp chiếu, hoặc Diễn viên. Sử dụng các thẻ Tag nhanh (Hành động, Kinh dị, Tình cảm) dưới thanh tìm kiếm để người dùng bấm chọn nhanh.

#### [MODIFY] [MovieDetails.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/MovieDetails.razor)
* **Chức năng**: Trang thông tin chi tiết phim.
* **Tiêu chuẩn UX**:
  * Backdrop mờ nghệ thuật lấy từ poster chính của phim.
  * Hiển thị điểm số đánh giá từ cộng đồng (User Rating) song song với điểm chuyên môn (nếu có).
  * Tab phân vùng: **Thông tin** (nội dung, đạo diễn, diễn viên), **Đánh giá** (danh sách bình luận có gắn tag cảm xúc), và **Lịch chiếu** (truy cập nhanh).
  * Nút "ĐẶT VÉ NGAY" cố định dạng nổi (Floating action button) dễ bấm bằng ngón tay cái.

#### [NEW] [ShowtimeSelection.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/ShowtimeSelection.razor)
* **Chức năng**: Chọn Rạp chiếu, Ngày chiếu & Suất chiếu.
* **Tiêu chuẩn UX (AMC/CGV style)**:
  * **Thanh tiến trình đặt vé (Booking Stepper)** ở trên cùng (`Chọn Vé -> Chọn Ghế -> Bắp Nước -> Thanh Toán`) giúp người dùng biết mình đang ở bước nào.
  * Bộ chọn ngày dạng thanh cuộn ngang (Horizontal Date Picker) hiển thị Thứ, Ngày và ưu đãi giá vé (ví dụ: Happy Day).
  * Bộ lọc rạp theo khoảng cách địa lý (GPS) hoặc rạp yêu thích (Favorite Theaters).
  * Phân loại suất chiếu rõ ràng theo định dạng công nghệ phòng: `2D Standard`, `3D Gold Class`, `IMAX`, `4DX`.

#### [NEW] [SeatSelection.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/SeatSelection.razor)
* **Chức năng**: Sơ đồ ghế ngồi phòng rạp.
* **Tiêu chuẩn UX**:
  * Curved screen (màn hình cong phát sáng) giả lập thực tế ở trên cùng phòng chiếu.
  * Hỗ trợ zoom bằng cử chỉ (Pinch-to-zoom) cho các rạp có quy mô lớn (200-300 ghế).
  * Phân biệt rõ ràng các loại ghế: Ghế đơn thường, Ghế VIP (màu sắc nổi bật), Ghế đôi Sweetbox (đặt ở hàng cuối cùng).
  * Tích hợp đồng hồ đếm ngược giữ ghế (10 phút) để tránh hiện tượng giữ chỗ ảo.

#### [NEW] [ConcessionsSelection.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/ConcessionsSelection.razor)
* **Chức năng**: Màn hình chọn bắp nước đi kèm (F&B / Concessions Up-sell).
* **Tiêu chuẩn UX**:
  * Trình bày dạng danh sách thực đơn trực quan: ảnh bắp nước ngon mắt, mô tả vị (bơ, phô mai, caramel), giá tiền.
  * Tách biệt các gói Combo (Combo Solo, Combo Couple, Combo Gia đình) giúp người dùng dễ lựa chọn và tiết kiệm chi phí.
  * Nút "Bỏ qua bước này" (Skip) ở góc phải để khách hàng không có nhu cầu thanh toán nhanh hơn.

#### [NEW] [Checkout.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Checkout.razor)
* **Chức năng**: Trang thanh toán & Hóa đơn.
* **Tiêu chuẩn UX**:
  * Tóm tắt chi tiết đơn hàng trực quan (Tên phim, Rạp, Ghế, Combo Bắp nước, Tổng tiền).
  * Ô nhập mã giảm giá (Promo code/Voucher) và ô áp dụng điểm thưởng thành viên.
  * Phương thức thanh toán một chạm: Tích hợp Apple Pay, Google Pay, Ví điện tử (MoMo, ZaloPay), Thẻ nội địa/quốc tế.

#### [MODIFY] [BookingSuccess.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/BookingSuccess.razor)
* **Chức năng**: Thông báo thanh toán thành công.
* **Tiêu chuẩn UX**:
  * Hiệu ứng pháo hoa chúc mừng trực quan.
  * Hiển thị mã vé (Booking ID) và mã QR Code lớn.
  * Nút "Thêm vào Apple/Google Wallet" và "Lưu vé vào Thư viện ảnh" để quét ngoại tuyến.
  * Tích hợp nút "Thêm vào lịch cá nhân (Add to Calendar)" để điện thoại tự động nhắc nhở trước giờ chiếu 30 phút.

#### [MODIFY] [MyTickets.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/MyTickets.razor)
* **Chức năng**: Danh sách vé đã mua và lịch sử.
* **Tiêu chuẩn UX**:
  * Quản lý vé thông minh, hỗ trợ hiển thị ngoại tuyến (offline cache) khi người dùng ở trong rạp không có sóng mạng.
  * Phân biệt rõ vé "Sắp diễn ra" (màu sáng nổi bật) và vé "Lịch sử" (màu trầm/bị mờ).

#### [NEW] [TicketDetails.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/TicketDetails.razor)
* **Chức năng**: Chi tiết tấm vé điện tử.
* **Tiêu chuẩn UX**: Thiết kế giao diện giống một tấm vé giấy truyền thống có đường cắt xé rách nghệ thuật (Cinema Ticket look), hiển thị QR Code động có khả năng chống chụp màn hình gian lận (tự động cập nhật mã sau mỗi 30 giây nếu cần bảo mật cao).

#### [MODIFY] [Profile.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Profile.razor)
* **Chức năng**: Quản lý tài khoản cá nhân.
* **Tiêu chuẩn UX**:
  * Hiển thị **Thẻ Thành Viên Điện Tử** (Loyalty Card) kèm mã vạch để tích điểm trực tiếp tại quầy bắp nước của rạp.
  * Thống kê số lượng phim đã xem trong năm (Gamification - ví dụ: danh hiệu "Mọt phim", "Chuyên gia điện ảnh").

#### [NEW] [WriteReviewDialog.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Components/WriteReviewDialog.razor)
* **Chức năng**: Hộp thoại viết đánh giá phim.
* **Tiêu chuẩn UX**: Cho phép người dùng chấm sao nhanh (1-5 sao) kèm chọn các tag cảm xúc nhanh (#KịchTính, #CảmĐộng, #KỹXảoĐỉnh, #NộiDungHơiChậm) giúp quá trình viết review trên điện thoại cực nhanh và không cần gõ bàn phím nhiều.

---

### 2. Phân Hệ Quản Trị (Admin Panel Screens)

Các màn hình quản lý nghiệp vụ rạp chiếu trong `src/WebFrontend.Shared/Pages/Admin/`.

#### [MODIFY] [Dashboard.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Admin/Dashboard.razor)
* **Chức năng**: Thống kê hiệu suất rạp.
* **Cải tiến Mobile UX**: Biểu đồ dạng thẻ rút gọn, lọc nhanh doanh số theo Rạp, theo Phim hoặc theo Ngày chiếu.

#### [MODIFY] [Movies.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Admin/Movies.razor)
* **Chức năng**: Thêm/Sửa/Xóa phim (quản lý metadata phim).

#### [MODIFY] [Shows.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Admin/Shows.razor)
* **Chức năng**: Quản lý và sắp xếp lịch chiếu phòng rạp.

#### [MODIFY] [Theaters.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Admin/Theaters.razor)
* **Chức năng**: Thiết lập rạp chiếu và cấu hình sơ đồ ghế ngồi.

#### [MODIFY] [Users.razor](file:///home/dinh/Rate/src/WebFrontend.Shared/Pages/Admin/Users.razor)
* **Chức năng**: Quản lý phân quyền tài khoản người dùng và xem hoạt động.

---

### 3. Cấu Trúc Nền Tảng MAUI Native (Hosting Shell - `src/MobileApp`)

Các tệp khởi chạy và thiết lập gốc của thiết bị di động.

#### [NEW] [MobileApp.csproj](file:///home/dinh/Rate/src/MobileApp/MobileApp.csproj)
* **Chức năng**: Định cấu hình SDK .NET 10, target nền tảng và các gói thư viện cài đặt.

#### [NEW] `MauiProgram.cs`
* **Chức năng**: Điểm khởi chạy của ứng dụng, đăng ký dịch vụ `BlazorWebView`, thiết lập Dependency Injection đồng bộ với Service Discovery của Aspire.

#### [NEW] `App.xaml` & `App.xaml.cs`
* **Chức năng**: Quản lý vòng đời ứng dụng di động.

#### [NEW] `MainPage.xaml` & `MainPage.xaml.cs`
* **Chức năng**: Trang giao diện gốc chứa điều khiển `BlazorWebView` liên kết tới Router.

#### [NEW] `Platforms/Android/`
* **Chức năng**: Định cấu hình quyền Android (`AndroidManifest.xml`) để định vị vị trí (GPS), camera và lưu trữ ngoại tuyến.

---

## Verification Plan

### Automated Tests
- Chạy toàn bộ test suites trong thư mục `tests/` để đảm bảo API và CQRS Command Handler vận hành chính xác với các tham số mới từ màn hình Bắp Nước (F&B) và chọn rạp theo GPS.

### Manual Verification
- Cài đặt app lên thiết bị Android hoặc Emulator.
- Thực hiện kiểm tra luồng đặt vé từ đầu đến cuối: Chọn Phim ➔ Chọn Suất Chiếu theo GPS Rạp ➔ Chọn Ghế ➔ Chọn Bắp Nước ➔ Thực hiện thanh toán giả làm ➔ Nhận vé QR Code và kiểm tra khả năng mở xem ngoại tuyến.
- Đăng nhập quyền Admin trên thiết bị và kiểm tra xem Dashboard quản trị hiển thị tương thích tốt trên điện thoại di động hay không.
