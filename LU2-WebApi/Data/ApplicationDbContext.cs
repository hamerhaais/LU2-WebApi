using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LU2_WebApi.Models;

namespace LU2_WebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<Environment2D> Environments => Set<Environment2D>();
        public DbSet<Object2D> Objects => Set<Object2D>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Zorgt ervoor dat EF begrijpt dat Environment2D een FK heeft naar ApplicationUser
            builder.Entity<Environment2D>()
                .HasOne(e => e.User)
                .WithMany() // Als je later een lijst met werelden per user wil: .WithMany(u => u.Environments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // of .Restrict als je dat liever wil
            
            builder.Entity<Object2D>()
                .HasOne(o => o.Environment)
                .WithMany()
                .HasForeignKey(o => o.EnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
