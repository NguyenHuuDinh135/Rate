namespace WebUI.Shared.Utils;

using System.Globalization;

public static class Formatter
{
    private static readonly CultureInfo VietnameseCulture = new CultureInfo("vi-VN");

    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C0", VietnameseCulture);
    }

    public static string FormatCurrency(double amount)
    {
        return ((decimal)amount).ToString("C0", VietnameseCulture);
    }

    public static string FormatDate(DateTime date, string format = "dd/MM/yyyy")
    {
        return date.ToString(format);
    }

    public static string FormatDate(string? dateString, string format = "dd/MM/yyyy")
    {
        if (string.IsNullOrEmpty(dateString)) return "--";
        if (DateTime.TryParse(dateString, out var date))
        {
            return date.ToString(format);
        }
        return dateString;
    }

    public static string FormatDateTime(DateTime date, string format = "dd/MM/yyyy HH:mm")
    {
        return date.ToString(format);
    }

    public static string FormatDateTime(string? dateTimeString, string format = "dd/MM/yyyy HH:mm")
    {
        if (string.IsNullOrEmpty(dateTimeString)) return "--";
        if (DateTime.TryParse(dateTimeString, out var date))
        {
            return date.ToString(format);
        }
        return dateTimeString;
    }

    public static string FormatTime(string? timeString)
    {
        if (string.IsNullOrEmpty(timeString)) return "--";
        // React app often uses "HH:mm:ss" from backend
        if (TimeSpan.TryParse(timeString, out var time))
        {
            return time.ToString(@"hh\:mm");
        }
        return timeString;
    }
}
