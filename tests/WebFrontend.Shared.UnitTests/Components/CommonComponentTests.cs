using Microsoft.AspNetCore.Components;
using Xunit;
using WebUI.Shared.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using WebUI.Shared.Models.Common;
using WebUI.Shared.Models.Payments;
using WebUI.Shared.Models.Movies;
using WebUI.Shared.Store.Auth;
using Moq;
using Fluxor;

namespace WebUI.Shared.UnitTests.Components;

public class CommonComponentTests : BunitContext
{
    public CommonComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact]
    public void StatCard_Should_RenderParameters()
    {
        var cut = Render<StatCard>(parameters => parameters
            .Add(p => p.Title, "Revenue")
            .Add(p => p.Value, "100k")
            .Add(p => p.Delta, "+10%")
            .Add(p => p.Icon, new Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size24.History())
        );

        cut.Find("p").TextContent.ShouldContain("Revenue");
        cut.Find("h4").TextContent.ShouldContain("100k");
        cut.Find("span").TextContent.ShouldContain("+10%");
    }

    [Fact]
    public void DateSelector_Should_RenderDates()
    {
        var selectedDate = DateTime.Now;
        var cut = Render<DateSelector>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.DatesWithShows, new List<string> { selectedDate.ToString("yyyy-MM-dd") })
        );

        var buttons = cut.FindAll("button");
        buttons.Count.ShouldBeGreaterThanOrEqualTo(7);
    }

    [Fact]
    public void TicketListItem_Should_RenderPaymentInfo()
    {
        var payment = new PaymentDto { 
            PaymentId = 1, 
            Amount = 190000, 
            PaymentMethod = "card",
            Movie = new PaymentMovieDto { Title = "Interstellar", PosterUrl = "" }
        };

        var cut = Render<TicketListItem>(parameters => parameters
            .Add(p => p.Payment, payment)
        );

        cut.Find("h3").TextContent.ShouldContain("Interstellar");
    }

    [Fact]
    public void PermissionGate_Should_HideContent_When_NoPermission()
    {
        var mockState = new Mock<IState<AuthState>>();
        mockState.Setup(s => s.Value).Returns(new AuthState(true, false, null, null, new List<string>()));
        Services.AddSingleton(mockState.Object);

        var cut = Render<PermissionGate>(parameters => parameters
            .Add(p => p.Permission, "Admin.Access")
            .AddChildContent("<div id='secret'>Secret</div>")
        );

        cut.FindAll("#secret").ShouldBeEmpty();
    }

    [Fact]
    public void PermissionGate_Should_ShowContent_When_HasPermission()
    {
        var mockState = new Mock<IState<AuthState>>();
        mockState.Setup(s => s.Value).Returns(new AuthState(true, false, null, null, new List<string> { "Admin.Access" }));
        Services.AddSingleton(mockState.Object);

        var cut = Render<PermissionGate>(parameters => parameters
            .Add(p => p.Permission, "Admin.Access")
            .AddChildContent("<div id='secret'>Secret</div>")
        );

        cut.Find("#secret").ShouldNotBeNull();
    }
}
