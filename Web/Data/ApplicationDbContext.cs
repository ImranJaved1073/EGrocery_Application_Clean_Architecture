using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
