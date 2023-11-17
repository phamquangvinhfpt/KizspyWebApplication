using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.Models;
namespace KizspyWebApp.Models
{
    public class KizspyDbContext : IdentityDbContext<AppUser>
{
    public KizspyDbContext(DbContextOptions<KizspyDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
    .HasOne(e => e.AppUser)
    .WithOne();

        modelBuilder.Entity<Cart>()
    .HasMany(e => e.cartItems)
    .WithOne(e => e.Cart)
    .HasForeignKey(e => e.CartId)
        .IsRequired();

        modelBuilder.Entity<Product>()
    .HasMany(e => e.Categories)
    .WithMany(e => e.Products)
    .UsingEntity<ProductCategory>();
    }
}
}