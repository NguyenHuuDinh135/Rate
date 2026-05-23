using Microsoft.AspNetCore.Components;
using Xunit;
using WebUI.Shared.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace WebUI.Shared.UnitTests.Components;

public class DataStateTests : BunitContext
{
    public DataStateTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact]
    public void Should_RenderLoading_When_IsLoadingIsTrue()
    {
        var loadingMessage = "Testing Loading...";
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.LoadingMessage, loadingMessage)
        );

        cut.Find("span").MarkupMatches($"<span class=\"text-xs font-black uppercase tracking-widest text-slate-400\">{loadingMessage}</span>");
    }

    [Fact]
    public void Should_RenderError_When_ErrorIsNotEmpty()
    {
        var errorMessage = "Something went wrong";
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.Error, errorMessage)
        );

        cut.Find("p").MarkupMatches($"<p class=\"text-sm text-slate-500 font-medium leading-relaxed\">{errorMessage}</p>");
    }

    [Fact]
    public void Should_CallOnRetry_When_RetryButtonClicked()
    {
        var retryCalled = false;
        var cut = Render<DataState>(parameters => parameters
            .Add(p => p.Error, "Error")
            .Add(p => p.OnRetry, () => retryCalled = true)
        );

        cut.Find("fluent-button").Click();
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

        cut.Find("p").MarkupMatches($"<p class=\"text-sm text-slate-500 font-medium italic\">{emptyMessage}</p>");
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
