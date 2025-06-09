using Microsoft.EntityFrameworkCore;
using EHS.Models;

namespace EHS.Data
{
    public class MOCContext : DbContext
    {
        public MOCContext(DbContextOptions<MOCContext> options)
    : base(options)
        {

        }

        public DbSet<__mst_employee>? __mst_employee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<__mst_employee>();
            modelBuilder.Entity<__mst_employee>().Metadata.SetIsTableExcludedFromMigrations(true);
        }
    }
}