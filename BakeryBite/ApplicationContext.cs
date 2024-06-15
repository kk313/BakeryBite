using BakeryBite.Models;

using Microsoft.EntityFrameworkCore;

namespace BakeryBite
{
    public class ApplicationContext : DbContext
    {
        private readonly string _connection = "Data Source=DESKTOP-S1JLV2G\\SQLEXPRESS;Database=BakeryBite;Integrated Security=sspi;Encrypt=true;TrustServerCertificate=true;";
        //private readonly string _connection = "Data Source=DESKTOP-S1JLV2G\\SQLEXPRESS;Initial Catalog=BakeryBite;Integrated Security=True";

        public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)             
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connection);
        }
    }
}
