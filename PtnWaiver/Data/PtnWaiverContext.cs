using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Models;

namespace PtnWaiver.Data
{
    public class PtnWaiverContext : DbContext
    {
        public PtnWaiverContext (DbContextOptions<PtnWaiverContext> options)
            : base(options)
        {
        }

        public DbSet<PtnWaiver.Models.Administrators>? Administrators { get; set; }
        public DbSet<PtnWaiver.Models.PTN> PTN { get; set; } = default!;
        public DbSet<PtnWaiver.Models.Waiver>? Waiver { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        public DbSet<PtnWaiver.Models.PtnStatus>? PtnStatus { get; set; }
        public DbSet<PtnWaiver.Models.BouleSize>? BouleSize { get; set; }
        public DbSet<PtnWaiver.Models.SubjectType>? SubjectType { get; set; }
        public DbSet<PtnWaiver.Models.Group>? Group { get; set; }
        public DbSet<PtnWaiver.Models.GroupApprovers>? GroupApprovers { get; set; }
        public DbSet<PtnWaiver.Models.WaiverStatus>? WaiverStatus { get; set; }
        public DbSet<PtnWaiver.Models.PorProject>? PorProject { get; set; }
        public DbSet<PtnWaiver.Models.OriginatingGroup>? OriginatingGroup { get; set; }
        public DbSet<PtnWaiver.Models.ProductProcess>? ProductProcess { get; set; }
        public DbSet<PtnWaiver.Models.AllowedAttachmentExtensions>? AllowedAttachmentExtensions { get; set; }
        public DbSet<PtnWaiver.Models.EmailHistory>? EmailHistory { get; set; }
    }
}
