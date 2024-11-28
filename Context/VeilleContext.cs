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
    }
}