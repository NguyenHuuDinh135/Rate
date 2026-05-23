using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace backend.Application.AI.Plugins;

public class SystemInfoPlugin
{
    [KernelFunction, Description("Lấy thời gian hiện tại của hệ thống")]
    public string GetSystemTime()
    {
        return DateTime.Now.ToString("F");
    }

    [KernelFunction, Description("Chào người dùng")]
    public string GreetUser(string name)
    {
        return $"Xin chào {name}, tôi có thể giúp gì cho bạn?";
    }
}
