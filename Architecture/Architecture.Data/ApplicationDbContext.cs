using Architecture.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Architecture.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext()
        {
                
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectUser>(option =>
            {
                option.HasKey(x => new { x.ProjectId, x.UserId });
            });

            builder.Entity<User>()
                .HasMany(x=>x.ProjectUsers)
                .WithOne(x=>x.User)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                .HasMany(x=>x.ProjectUsers)
                .WithOne(x=>x.Project)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public virtual DbSet<Address> Addresses{ get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Town> Towns { get; set; }

        public virtual DbSet<ProjectUser> ProjectsUsers { get; set; }



    }
}
