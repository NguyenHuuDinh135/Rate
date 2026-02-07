using System.Diagnostics;

internal static class ActivityExtensions
{
    // Extension method để gắn thông tin Exception vào Activity (trace)
    // Tuân theo OpenTelemetry semantic conventions
    public static void SetExceptionTags(this Activity activity, Exception ex)
    {
        // Nếu activity không tồn tại thì bỏ qua
        if (activity is null)
        {
            return;
        }

        // Message của exception
        activity.AddTag("exception.message", ex.Message);

        // Stacktrace đầy đủ
        activity.AddTag("exception.stacktrace", ex.ToString());

        // Loại exception (System.InvalidOperationException, ...)
        activity.AddTag("exception.type", ex.GetType().FullName);

        // Đánh dấu span này là lỗi
        activity.SetStatus(ActivityStatusCode.Error);
    }
}