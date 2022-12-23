using Jobsway2goMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static System.Collections.Specialized.BitVector32;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Jobsway2goMvc.Models.ViewModel;
using System.Diagnostics;

namespace Jobsway2goMvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<Event> Events { get; set; }

        //public DbSet<Notification> Notifications { get; set; }
        //public DbSet<Section> Sections { get; set; }
        //public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configures one-to-many relationship
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Job>()
                .HasOne<Collection>(s => s.Collection)
                .WithMany(g => g.Jobs)
                .HasForeignKey(s => s.CollectionId);
        }
    }
}
