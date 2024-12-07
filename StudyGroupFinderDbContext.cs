using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyGroupFinder.Models;

namespace StudyGroupFinder
{
    public class StudyGroupFinderDbContext : IdentityDbContext<ApplicationUser>
    {
        public StudyGroupFinderDbContext(DbContextOptions<StudyGroupFinderDbContext> options) : base(options) {}

        // Register tables
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure this is called first

            // Optional: Customize the Identity entities' configuration
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }); // Set composite key for IdentityUserLogin
            });
            modelBuilder.Entity<Seller>().HasData(
                new Seller()
                {
                    Id = 1,
                    Name = "Emall",
                },
                new Seller()
                {
                    Id = 2,
                    Name = "SM",
                }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    Name = "Chicharon",
                    Price = 123.32f,
                    SellerId = 1,
                },
            new Product()
            {
                Id = 2,
                Name = "Adobo",
                Price = 123.32f,
                SellerId = 1,
            },
            new Product()
            {
                Id = 3,
                Name = "Ginaling",
                Price = 123.32f,
                SellerId = 2,
            }
                );
        }
    }
}
