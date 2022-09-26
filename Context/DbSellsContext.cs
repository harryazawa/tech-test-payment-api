using Microsoft.EntityFrameworkCore;
using tech-test-payment-api.Entities;
namespace DefaultNamespace;

public class DbSellsContext : DbContext 
{
    public DbSellsContext(DbContextOptions<DbSellsContext> options) : base(options)
    { }
    public DbSet<Seller> OrderSellers { get; set; }
    public DbSet<Order> OrdersTable { get; set;}
}