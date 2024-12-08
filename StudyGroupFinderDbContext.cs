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
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }
        public DbSet<SessionParticipant> SessionParticipants { get; set; }
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
            // Configure relationships for GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.UserId, gm.StudyGroupId });

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.StudyGroup)
                .WithMany(sg => sg.Members)
                .HasForeignKey(gm => gm.StudyGroupId);

            // Optional: Configure relationships for StudySession
            modelBuilder.Entity<SessionParticipant>()
                .HasKey(sp => new { sp.UserId, sp.StudySessionId });

            modelBuilder.Entity<SessionParticipant>()
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId);

            modelBuilder.Entity<SessionParticipant>()
                .HasOne(sp => sp.StudySession)
                .WithMany(ss => ss.Participants)
                .HasForeignKey(sp => sp.StudySessionId);
        }

    }
}
