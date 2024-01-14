using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using clothing_shop.Models;

namespace clothing_shop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Size> Size { get; set; } = default!;
        public DbSet<Colors> Colors { get; set; } = default!;
        public DbSet<ProductSize> ProductSizes { get; set; } = default!;
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductSize>()
                .HasKey(ps => new { ps.ProductId, ps.SizeId });

            modelBuilder.Entity<ProductSize>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductSize>()
                .HasOne(ps => ps.Size)
                .WithMany(/*s => s.ProductSizes*/)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
