using WebUI.Shared.Models.Auth;

namespace WebUI.Shared.Services.Admin;

public class AdminMockDataService
{
    public List<UserProfileDto> GetUsers() =>
    [
        new("1", "admin", "admin@rate.local", ["Permission.ManagePermissions"]),
        new("2", "manager", "manager@rate.local", ["Permission.Movies.Edit", "Permission.Bookings.Edit"]),
        new("3", "staff", "staff@rate.local", ["Permission.Bookings.Edit"])
    ];

    public List<UserActivityDto> GetActivities() =>
    [
        new("admin", "Cập nhật quyền nhóm Manager", "2 phút trước", "Security"),
        new("manager", "Thêm suất chiếu mới", "18 phút trước", "Showtime"),
        new("staff", "Hủy một đặt vé", "45 phút trước", "Booking"),
        new("admin", "Cập nhật trạng thái phim", "1 giờ trước", "Movie")
    ];
}

public record UserActivityDto(string UserName, string Action, string TimeAgo, string Type);
