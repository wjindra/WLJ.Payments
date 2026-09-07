using Microsoft.EntityFrameworkCore;

namespace WLJ.Payments.Migrations;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
}
