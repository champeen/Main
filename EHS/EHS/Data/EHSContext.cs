using EHS.Models;
using EHS.Models.Dropdowns;
using Microsoft.EntityFrameworkCore;

//namespace EHS.Models.Dropdowns;

namespace EHS.Data
{
    public class EHSContext : DbContext
    {
        public EHSContext(DbContextOptions<EHSContext> options)
            : base(options) { }

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

            modelBuilder.Entity<seg_risk_assessment>()
            .Property(e => e.risk_score)
            .HasComputedColumnSql("\"exposure_rating\" * \"health_effect_rating\"", stored: true);

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
        public DbSet<EHS.Models.Dropdowns.monitoring_data_required> monitoring_data_required { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.controls_recommended> controls_recommended { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.yes_no> yes_no { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.assessment_methods_used> assessment_methods_used { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.number_of_workers> number_of_workers { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.has_agent_been_changed> has_agent_been_changed { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.exposure_rating> exposure_rating { get; set; } = default!;
        public DbSet<EHS.Models.Dropdowns.health_effect_rating> health_effect_rating { get; set; } = default!;
    }
}
