using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DefaultCategory> DefaultCategories { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Theme> Themes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(admin => admin.Username)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(category => new { category.MenuId, category.Name })
                .IsUnique();

            modelBuilder.Entity<DefaultCategory>()
                .HasIndex(category => category.Name)
                .IsUnique();

            modelBuilder.Entity<Restaurant>()
                .HasIndex(restaurant => restaurant.Slug)
                .IsUnique()
                .HasFilter(null);
        }

        }
}
