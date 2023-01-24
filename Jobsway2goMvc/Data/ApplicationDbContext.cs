using Jobsway2goMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static System.Collections.Specialized.BitVector32;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Jobsway2goMvc.Models.ViewModel;

namespace Jobsway2goMvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

      
        public DbSet<Post> Posts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventGuest> EventGuests { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HubConnection> HubConnections { get; set; }
        public DbSet<Report> Reports { get; set; }

        public DbSet<Connection> Connections { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Education> Educations { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Award> Awards { get; set; }
        //public DbSet<Section> Sections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .HasOne(p => p.Category)
                .WithMany(g => g.Jobs)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(p => p.GroupId);

            modelBuilder.Entity<ApplicationUser>()
                 .HasMany(p => p.Jobs)
                 .WithMany(p => p.Applicants)
                 .UsingEntity(j => j.ToTable("JobApplicants"));

            modelBuilder
                .Entity<Collection>()
                .HasMany(p => p.Jobs)
                .WithMany(p => p.Collections)
                .UsingEntity(j => j.ToTable("JobCollections"));
            modelBuilder.Entity<Experience>()
                .HasOne(p => p.User)
                .WithMany(g => g.Experiences)
                .HasForeignKey(p => p.UserId);
            modelBuilder.Entity<Education>()
                .HasOne(p => p.User)
                .WithMany(g => g.Educations)
                .HasForeignKey(p => p.UserId);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Certification>()
                .HasOne(p => p.User)
                .WithMany(g => g.Certifications)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<EventGuest>()
                .HasKey(eg => new { eg.EventId, eg.GuestId });

            modelBuilder.Entity<EventGuest>()
                .HasOne(eg => eg.Event)
                .WithMany(e => e.EventGuests)
                .HasForeignKey(eg => eg.EventId);

            modelBuilder.Entity<EventGuest>()
                .HasOne(eg => eg.ApplicationUser)
                .WithMany(u => u.EventGuests)
                .HasForeignKey(eg => eg.GuestId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
