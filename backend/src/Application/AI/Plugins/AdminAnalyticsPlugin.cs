using System.ComponentModel;
using backend.Application.Common.Interfaces;
using backend.Domain.Constants;
using backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace backend.Application.AI.Plugins;

public class AdminAnalyticsPlugin(IApplicationDbContext dbContext, IUser user)
{
    private void EnsureAdmin()
    {
        if (user.Roles == null || !user.Roles.Contains(Roles.Administrator))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu phân tích.");
        }
    }

    [KernelFunction("get_total_revenue")]
    [Description("Lấy tổng doanh thu từ tất cả các vé đã đặt thành công. Chỉ dành cho Admin.")]
    public async Task<string> GetTotalRevenue()
    {
        EnsureAdmin();
        var total = await dbContext.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .SumAsync(b => b.Price);

        return $"Tổng doanh thu hiện tại là: {total:N0} VNĐ";
    }

    [KernelFunction("get_booking_statistics")]
    [Description("Lấy thống kê số lượng đặt vé. Chỉ dành cho Admin.")]
    public async Task<string> GetBookingStatistics()
    {
        EnsureAdmin();
        var total = await dbContext.Bookings.CountAsync();
        var success = await dbContext.Bookings.CountAsync(b => b.Status != BookingStatus.Cancelled);
        var cancelled = total - success;

        return $"Thống kê đặt vé:\n- Tổng số: {total}\n- Thành công: {success}\n- Đã hủy: {cancelled}";
    }

    [KernelFunction("get_most_popular_movie")]
    [Description("Tìm bộ phim có số lượng đặt vé cao nhất. Chỉ dành cho Admin.")]
    public async Task<string> GetMostPopularMovie()
    {
        EnsureAdmin();
        var stats = await dbContext.Bookings
            .GroupBy(b => b.Show.Movie.Title)
            .Select(g => new { Title = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        if (stats == null) return "Chưa có dữ liệu đặt vé.";

        return $"Bộ phim phổ biến nhất là '{stats.Title}' với {stats.Count} lượt đặt vé.";
    }

    [KernelFunction("get_occupancy_rate")]
    [Description("Lấy tỷ lệ lấp đầy ghế của một rạp hoặc suất chiếu. Chỉ dành cho Admin.")]
    public async Task<string> GetOccupancyRate(
        [Description("ID của suất chiếu (ShowId)")] int showId)
    {
        EnsureAdmin();
        var show = await dbContext.Shows
            .Include(s => s.Theater)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null) return "Không tìm thấy suất chiếu.";

        int totalSeats = show.Theater.NumOfRows * show.Theater.SeatsPerRow;
        int occupiedSeats = show.Bookings.Count(b => b.Status != BookingStatus.Cancelled);
        double rate = (double)occupiedSeats / totalSeats * 100;

        return $"Tỷ lệ lấp đầy cho suất chiếu ID {showId} là {rate:F2}% ({occupiedSeats}/{totalSeats} ghế).";
    }
}
