namespace WebUI.Shared.Services.Admin;

public interface IAdminThemeService
{
    bool IsDarkMode { get; }
    event Action? ThemeChanged;
    void SetDarkMode(bool isDarkMode);
    void Toggle();
}
