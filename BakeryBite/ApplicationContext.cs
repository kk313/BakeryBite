﻿using BakeryBite.Models;

using Microsoft.EntityFrameworkCore;

namespace BakeryBite
{
    public class ApplicationContext : DbContext
    {
        private readonly string _connection = "Data Source=DESKTOP-4PAD45N\\SQLEXPRESS;Initial Catalog=BakeryBite;Integrated Security=True";

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Category> Category { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(_connection);
        }
    }
}