using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using WLJ.Payments.Migrations;

namespace WLJ.Payments.Tests;

public class MigrationsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17")
        .Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    private PaymentsDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options);

    [Fact]
    public async Task All_migrations_apply_to_a_fresh_database()
    {
        await using var db = CreateContext();

        await db.Database.MigrateAsync();

        Assert.True(await db.Database.CanConnectAsync());
        var applied = await db.Database.GetAppliedMigrationsAsync();
        Assert.Equal(db.Database.GetMigrations(), applied);
    }
}
