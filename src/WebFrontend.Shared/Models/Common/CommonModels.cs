namespace WebFrontend.Shared.Models.Common;

public record ApiResponse<T>(ApiHeaders Headers, T Body);
public record ApiHeaders(int Success, string Message);

public enum MovieType { ComingSoon, NowShowing, Removed }
public enum PaymentMethod { Cash, Card, Cod }
public enum ShowType { ThreeD, TwoD }
public enum BookingStatus { Reserved, Paid, Cancelled }

public record PaymentDialogData(decimal TotalAmount, int SeatCount);
