using CoreMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CoreMvc.Data
{
    public class CoreMvcDbContext : DbContext
    {
        public CoreMvcDbContext(DbContextOptions<CoreMvcDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandModel>()
                .HasOne(m => m.Brand)
                .WithMany(m => m.BrandModels)
                .HasForeignKey(m => m.Brand_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(m => m.BrandModel)
                .WithMany(m => m.Products)
                .HasForeignKey(m => m.BrandModel_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(m => m.Brand)
                .WithMany(m => m.Products)
                .HasForeignKey(m => m.Brand_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(m => m.Employee)
                .WithOne(m => m.User)
                .HasForeignKey<User>(m => m.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(m => m.User)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(m => m.User_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(m => m.Role)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(m => m.Role_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "Admin" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 2, Name = "Human Resource" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 3, Name = "Stock" });

            modelBuilder.Entity<Employee>().HasData(new Employee { Id = 1, Name = "Admin", Surname = "Admin", GsmNo = "000000000", Email = "admin@coremvc.com", BirthDate = DateTime.Now });
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Employee_Id = 1, Username = "admin", Password = "1" });
            modelBuilder.Entity<UserRole>().HasData(new UserRole { Id = 1, Role_Id = 1, User_Id = 1 });
            modelBuilder.Entity<UserRole>().HasData(new UserRole { Id = 2, Role_Id = 2, User_Id = 1 });
            modelBuilder.Entity<UserRole>().HasData(new UserRole { Id = 3, Role_Id = 3, User_Id = 1 });

            //With this code, we can save Table values history. This must be defined.
            modelBuilder.EnableAutoHistory(null);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("CoreMvcDbContext"));
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandModel> BrandModels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }

    public static class Constants
    {
        public const string AdminRole = "Admin";
        public const string HumanResourceRole = "Human Resource";
        public const string StockRole = "Stock";
    }
}
