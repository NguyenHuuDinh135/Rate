using backend.Application.Common.Behaviours;
using backend.Application.Common.Interfaces;
using backend.Application.Movies.Queries.GetFilteredMovies;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace backend.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<GetFilteredMoviesQuery>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<GetFilteredMoviesQuery>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<GetFilteredMoviesQuery>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new GetFilteredMoviesQuery(null, null, null), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<GetFilteredMoviesQuery>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new GetFilteredMoviesQuery(null, null, null), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
