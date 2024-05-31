using Architecture.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Architecture.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
            builder.Entity<User>()
                .HasMany(p => p.ProjectUsers)
                .WithOne(u => u.User)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Project>()
              .HasMany(p => p.ProjectUsers)
              .WithOne(u => u.Project)
              .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Project>()
             .HasOne(a => a.Address)
             .WithOne(u => u.Project);
            base.OnModelCreating(builder);
        }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectUser> ProjectUsers { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
    }
}
