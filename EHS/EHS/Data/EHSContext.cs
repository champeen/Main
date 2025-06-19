using Microsoft.EntityFrameworkCore;
using EHS.Models.Dropdowns;

//namespace EHS.Models.Dropdowns;

namespace EHS.Data
{
    public class EHSContext : DbContext
    {
        public EHSContext (DbContextOptions<EHSContext> options)
            : base(options) {}        

        public override int SaveChanges()
        {
            //ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ConvertDateTimesToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<EHS.Models.seg_risk_assessment> seg_risk_assessment { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.location> location { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ehs");
            modelBuilder.HasAnnotation("Relational:HistoryTableSchema", "ehs");

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<EHS.Models.Dropdowns.exposure_type> exposure_type { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.agent> agent { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.seg_role> seg_role { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.task> task { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.occupational_exposure_limit> occupational_exposure_limit { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.acute_chronic> acute_chronic { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.route_of_entry> route_of_entry { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.frequency_of_task> frequency_of_task { get; set; } = default!;
    }
}
