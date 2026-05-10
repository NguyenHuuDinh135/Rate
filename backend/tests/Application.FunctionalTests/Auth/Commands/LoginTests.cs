using backend.Application.Auth.Commands.Login;
using backend.Application.FunctionalTests.Infrastructure;
using Shouldly;

namespace backend.Application.FunctionalTests.Auth.Commands;

using static TestApp;

public class LoginTests : TestBase
{
    [Test]
    public async Task ShouldLoginWithValidCredentials()
    {
        var userName = "testuser";
        var password = "Testing1234!";
        await RunAsUserAsync(userName, password, []);

        var command = new LoginCommand
        {
            Email = userName, // The app uses userName as Email in some places or vice versa
            Password = password
        };

        var result = await SendAsync(command);

        result.ShouldNotBeNull();
        result.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ShouldNotLoginWithInvalidPassword()
    {
        var userName = "testuser";
        await RunAsUserAsync(userName, "Testing1234!", []);

        var command = new LoginCommand
        {
            Email = userName,
            Password = "WrongPassword123!"
        };

        var result = await SendAsync(command);

        result.ShouldBeNull();
    }
}
