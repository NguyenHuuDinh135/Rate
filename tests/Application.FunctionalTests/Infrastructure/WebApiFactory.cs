using backend.Application.Common.Interfaces;
using backend.Application.Common.Interfaces.AI;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using StackExchange.Redis;

namespace backend.Application.FunctionalTests.Infrastructure;

public class WebApiFactory(string dbConnectionString, string messagingConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseSetting("ConnectionStrings:MovieDb", dbConnectionString)
            .UseSetting("ConnectionStrings:messaging", messagingConnectionString)
            .UseSetting("ConnectionStrings:redis", "localhost:6379");

        builder.ConfigureTestServices(services =>
        {
            // Mock Redis
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            mockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
            
            services.RemoveAll<IConnectionMultiplexer>();
            services.AddSingleton(mockMultiplexer.Object);

            // Mock ICacheService
            services.RemoveAll<ICacheService>();
            services.AddSingleton(Mock.Of<ICacheService>());

            // Mock MassTransit to use InMemory
            services.AddMassTransitTestHarness(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services
                .RemoveAll<IUser>()
                .AddTransient(provider =>
                {
                    var mock = new Mock<IUser>();
                    mock.SetupGet(x => x.Roles).Returns(TestApp.GetRoles());
                    mock.SetupGet(x => x.Id).Returns(TestApp.GetUserId());
                    return mock.Object;
                });
        });
    }
}
