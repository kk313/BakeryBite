using BakeryBite.Models;

using Microsoft.EntityFrameworkCore;

namespace BakeryBite
{
    public class ApplicationContext : DbContext
    {
		//private readonly string _connection = "Data Source=192.168.221.12;User ID = user04;Password=04;Database=BakeryBite;TrustServerCertificate=true";
        private readonly string _connection = "Data Source=DESKTOP-4PAD45N\\SQLEXPRESS;Database=BakeryBite;Integrated Security=sspi;Encrypt=true;TrustServerCertificate=true;";

        public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<CartItem> CartItem { get; set; }

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

		public ApplicationContext()
		{
            //Database.EnsureCreated();
            //Database.EnsureDeleted();
        }
    }
}
