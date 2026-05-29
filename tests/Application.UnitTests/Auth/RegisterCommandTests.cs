using backend.Application.Auth.Commands.Register;
using backend.Application.Common.Models;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace backend.Application.UnitTests.Auth;

public class RegisterCommandTests
{
    [Test]
    public async Task Handle_ShouldReturnFailure_WhenIdentityCreateFails()
    {
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(x => x.CreateUserAsync("Test User", "test@example.com", "weak"))
            .ReturnsAsync((Result.Failure(["Password requires a digit."]), string.Empty));

        var handler = new RegisterCommand(identityService.Object);

        var result = await handler.Handle(
            new RegisterRequest
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "weak"
            },
            CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain("Password requires a digit.");
    }
}
