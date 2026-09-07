using System.Net;
using Aspire.Hosting.Testing;

namespace WLJ.Payments.Tests;

public class AppHostSmokeTests
{
    [Fact]
    public async Task Server_health_endpoints_report_healthy()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.WLJ_Payments_AppHost>();

        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        await app.ResourceNotifications
            .WaitForResourceHealthyAsync("server")
            .WaitAsync(TimeSpan.FromMinutes(3));

        using var client = app.CreateHttpClient("server");

        var health = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);

        var alive = await client.GetAsync("/alive");
        Assert.Equal(HttpStatusCode.OK, alive.StatusCode);
    }
}
