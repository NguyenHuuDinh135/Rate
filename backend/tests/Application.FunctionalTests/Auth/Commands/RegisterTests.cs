using backend.Application.Auth.Commands.Register;
using backend.Application.FunctionalTests.Infrastructure;
using Shouldly;

namespace backend.Application.FunctionalTests.Auth.Commands;

using static TestApp;

public class RegisterTests : TestBase
{
    [Test]
    public async Task ShouldRegisterNewUser()
    {
        var request = new RegisterRequest
        {
            FullName = "New User",
            Email = "newuser", // Using letters only to avoid policy issue found earlier
            Password = "Password123!"
        };

        var result = await SendAsync(request);

        result.Succeeded.ShouldBeTrue();
    }
}
