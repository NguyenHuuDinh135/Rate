using System.Collections.Generic;

namespace WebUI.Shared.Store.Bookings;

// Khởi tạo luồng đặt vé mới
public record StartBookingAction(int MovieId);

// Lựa chọn thông tin cơ bản
public record SelectShowtimeAction(int TheaterId, int ShowtimeId);

// Quản lý ghế
public record ToggleSeatAction(SeatSelectionItem Seat);
public record ClearSeatsAction();

// Quản lý bắp nước
public record UpdateConcessionQuantityAction(ConcessionSelectionItem Concession, int Quantity);
public record ClearConcessionsAction();

// Hủy toàn bộ phiên giao dịch
public record ResetBookingWorkflowAction();
