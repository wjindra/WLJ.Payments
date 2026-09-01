using Microsoft.EntityFrameworkCore;
using WLJ.Payments.Migrations.Models;

namespace WLJ.Payments.Migrations;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
}
