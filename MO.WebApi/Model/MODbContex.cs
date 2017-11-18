using Microsoft.EntityFrameworkCore;

public class MODbContext : DbContext
{
    public MODbContext(DbContextOptions<MODbContext> options)
        : base(options)
    { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderGroup> OrderGroup { get; set; }
}