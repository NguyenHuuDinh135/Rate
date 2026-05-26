namespace WebUI.Shared.Services.Admin;

public class AdminThemeService : IAdminThemeService
{
    public bool IsDarkMode { get; private set; } = true;
    public event Action? ThemeChanged;

    public void SetDarkMode(bool isDarkMode)
    {
        if (IsDarkMode == isDarkMode)
        {
            return;
        }

        IsDarkMode = isDarkMode;
        ThemeChanged?.Invoke();
    }

    public void Toggle()
    {
        SetDarkMode(!IsDarkMode);
    }
}
