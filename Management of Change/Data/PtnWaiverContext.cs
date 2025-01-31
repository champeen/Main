using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Models.PtnWaiver;

namespace Management_of_Change.Data
{
    public class PtnWaiverContext : DbContext
    {
        public PtnWaiverContext(DbContextOptions<PtnWaiverContext> options)
            : base(options)
        {
        }

        //public DbSet<PtnWaiver.Models.Administrators>? Administrators { get; set; }
        public DbSet<Management_of_Change.Models.PtnWaiver.PTN> PTN { get; set; } = default!;
        public DbSet<Management_of_Change.Models.PtnWaiver.Waiver>? Waiver { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Management_of_Change.Models.PtnWaiver.PTN>();
            modelBuilder.Entity<Management_of_Change.Models.PtnWaiver.PTN>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Ignore<Management_of_Change.Models.PtnWaiver.Waiver>();
            modelBuilder.Entity<Management_of_Change.Models.PtnWaiver.Waiver>().Metadata.SetIsTableExcludedFromMigrations(true);
        }
        //public DbSet<PtnWaiver.Models.PtnStatus>? PtnStatus { get; set; }
        //public DbSet<PtnWaiver.Models.BouleSize>? BouleSize { get; set; }
        //public DbSet<PtnWaiver.Models.SubjectType>? SubjectType { get; set; }
        //public DbSet<PtnWaiver.Models.Group>? Group { get; set; }
        //public DbSet<PtnWaiver.Models.GroupApprovers>? GroupApprovers { get; set; }
        //public DbSet<PtnWaiver.Models.WaiverStatus>? WaiverStatus { get; set; }
        //public DbSet<PtnWaiver.Models.PorProject>? PorProject { get; set; }
        //public DbSet<PtnWaiver.Models.OriginatingGroup>? OriginatingGroup { get; set; }
        //public DbSet<PtnWaiver.Models.ProductProcess>? ProductProcess { get; set; }
        //public DbSet<PtnWaiver.Models.AllowedAttachmentExtensions>? AllowedAttachmentExtensions { get; set; }
        //public DbSet<PtnWaiver.Models.EmailHistory>? EmailHistory { get; set; }
    }
}
