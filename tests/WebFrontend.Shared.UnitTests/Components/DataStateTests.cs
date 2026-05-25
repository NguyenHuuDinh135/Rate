using Microsoft.AspNetCore.Components;
using Xunit;
using WebUI.Shared.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using MudBlazor.Services;

namespace WebUI.Shared.UnitTests.Components;

public class DataStateTests : BunitContext
{
    public DataStateTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
        Services.AddMudServices();
    }

    [Fact]
    public void Should_RenderLoading_When_IsLoadingIsTrue()
    {
        var loadingMessage = "Testing Loading...";
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.LoadingMessage, loadingMessage)
        );

        cut.Find("span").TextContent.ShouldBe(loadingMessage);
    }

    [Fact]
    public void Should_RenderError_When_ErrorIsNotEmpty()
    {
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.Error, "Something went wrong")
        );

        cut.Find("h3").TextContent.ShouldContain("Không thể tải dữ liệu");
        cut.Markup.ShouldNotContain("Something went wrong");
    }

    [Fact]
    public void Should_CallOnRetry_When_RetryButtonClicked()
    {
        var retryCalled = false;
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.Error, "Error")
            .Add(p => p.OnRetry, () => retryCalled = true)
        );

        cut.Find("button").Click();
        retryCalled.ShouldBeTrue();
    }

    [Fact]
    public void Should_RenderEmpty_When_IsEmptyIsTrue()
    {
        var emptyMessage = "Nothing here";
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.IsEmpty, true)
            .Add(p => p.EmptyMessage, emptyMessage)
        );

        cut.Find("p").TextContent.ShouldBe(emptyMessage);
    }

    [Fact]
    public void Should_RenderChildContent_When_NoLoadingErrorOrEmpty()
    {
        var cut = Render<DataState>(parameters => parameters
            .AddChildContent("<div>Test Content</div>")
        );

        cut.MarkupMatches("<div>Test Content</div>");
    }
}
