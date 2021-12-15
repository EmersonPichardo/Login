using login_data_access.Contexts.SecurityContext.Models;
using Microsoft.EntityFrameworkCore;

namespace login_data_access.Contexts.SecurityContext
{
    public class SecurityContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Sesion> Sesions { get; set; }
        public DbSet<Application> Applications { get; set; }

        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new User.Configuration());
            modelBuilder.ApplyConfiguration(new Sesion.Configuration());
            modelBuilder.ApplyConfiguration(new Application.Configuration());
        }
    }
}
