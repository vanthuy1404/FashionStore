using FashionStore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Data
{
    public class FStoreDbContext : DbContext
    {
        public FStoreDbContext(DbContextOptions<FStoreDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // has name table oders
            modelBuilder.Entity<Order>()
                .ToTable("orders")
                .HasKey(o => o.id);


            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.product_id)
                .HasPrincipalKey(p => p.id); // id trong Product
            modelBuilder.Entity<CartItem>()
               .HasOne(c => c.User)
               .WithMany()
               .HasForeignKey(c => c.user_id)
               .HasPrincipalKey(p => p.id); // id trong User

        }
    }
}
