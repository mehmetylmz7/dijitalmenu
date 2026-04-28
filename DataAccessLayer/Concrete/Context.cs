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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DIDIM\\SQLEXPRESS;database=CoreBlogDb;integrated security=true");
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Theme> Themes { get; set; }
        public DbSet<User> Users { get; set; }


        }
}
