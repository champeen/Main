using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Models;

namespace PtnWaiver.Data
{
    public class MocContext : DbContext
    {
        public MocContext (DbContextOptions<MocContext> options)
            : base(options)
        {
        }

        public DbSet<PtnWaiver.Models.__mst_employee>? __mst_employee { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<__mst_employee>();
            modelBuilder.Entity<__mst_employee>().Metadata.SetIsTableExcludedFromMigrations(true);
        }
    }
}
