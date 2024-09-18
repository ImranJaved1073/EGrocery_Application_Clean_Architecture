using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EGrocerAPI
{
    public class MyDbContext : IdentityDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Product> Product { get; set; } = default!;
        public DbSet<Domain.Category> Category { get; set; } = default!;
        public DbSet<Domain.Brand> Brand { get; set; } = default!;
        public DbSet<Domain.Unit> Unit { get; set; } = default!;
        public DbSet<Domain.Orders> Order { get; set; } = default!;
        public DbSet<Domain.OrderDetail> OrderDetail { get; set; } = default!;
    }
}
