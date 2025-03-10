using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using VeilleApp.Model;

namespace VeilleApp.Context
{
    public class VeilleContext : DbContext
    {
        public DbSet<Veille> veilles { get; set; }

        public VeilleContext(DbContextOptions<VeilleContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Veille>()
                .HasIndex(e => e.Guid)
                .IsUnique();
        }
    }
}