using Microsoft.AspNetCore.Components;
using Xunit;
using WebUI.Shared.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using WebUI.Shared.Models.Common;
using WebUI.Shared.Models.Theaters;
using WebUI.Shared.Models.Bookings;

namespace WebUI.Shared.UnitTests.Components;

public class SeatMapTests : BunitContext
{
    public SeatMapTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact]
    public void Should_RenderCorrectNumberOfSeats()
    {
        var theater = new TheaterDto(1, "Main", 2, 2, TheaterType.Normal, new List<TheaterSeatDto>(), new List<TheaterSeatDto>());
        var cut = Render<SeatMap>(parameters => parameters
            .Add(p => p.Theater, theater)
        );

        cut.FindAll("button").Count.ShouldBe(4);
    }

    [Fact]
    public void Should_DisableBlockedSeats()
    {
        var theater = new TheaterDto(1, "Main", 1, 2, TheaterType.Normal, new List<TheaterSeatDto>(),
            new List<TheaterSeatDto> { new TheaterSeatDto("A", 1) });
        var cut = Render<SeatMap>(parameters => parameters
            .Add(p => p.Theater, theater)
        );

        var buttons = cut.FindAll("button");
        buttons[0].HasAttribute("disabled").ShouldBeTrue();
        buttons[1].HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void Should_TriggerOnToggleSeat_WhenValidSeatClicked()
    {
        var theater = new TheaterDto(1, "Main", 1, 1, TheaterType.Normal, new List<TheaterSeatDto>(), new List<TheaterSeatDto>());
        string clickedSeat = "";
        var cut = Render<SeatMap>(parameters => parameters
            .Add(p => p.Theater, theater)
            .Add(p => p.OnToggleSeat, (string seat) => clickedSeat = seat)
        );

        cut.Find("button").Click();
        clickedSeat.ShouldBe("A1");
    }
}
