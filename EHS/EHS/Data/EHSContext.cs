using EHS.Models;
using EHS.Models.IH;
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
        // --- NEW IH DbSets ---
        public DbSet<IhChemical> ih_chemical { get; set; } = default!;
        public DbSet<IhChemicalSynonym> ih_chemical_synonym { get; set; } = default!;
        public DbSet<IhChemicalProperty> ih_chemical_property { get; set; } = default!;
        public DbSet<IhChemicalHazard> ih_chemical_hazard { get; set; } = default!;
        public DbSet<IhChemicalOel> ih_chemical_oel { get; set; } = default!;
        public DbSet<IhChemicalSamplingMethod> ih_chemical_sampling_method { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ehs");
            modelBuilder.HasAnnotation("Relational:HistoryTableSchema", "ehs");

            modelBuilder.Entity<seg_risk_assessment>()
            .Property(e => e.risk_score)
            .HasComputedColumnSql("\"exposure_rating\" * \"health_effect_rating\"", stored: true);

            // --- IH tables mapping ---
            modelBuilder.Entity<IhChemical>().ToTable("ih_chemical");
            modelBuilder.Entity<IhChemicalSynonym>().ToTable("ih_chemical_synonym");
            modelBuilder.Entity<IhChemicalProperty>().ToTable("ih_chemical_property");
            modelBuilder.Entity<IhChemicalHazard>().ToTable("ih_chemical_hazard");
            modelBuilder.Entity<IhChemicalOel>().ToTable("ih_chemical_oel");
            modelBuilder.Entity<IhChemicalSamplingMethod>().ToTable("ih_chemical_sampling_method");


            modelBuilder.Entity<IhChemical>().HasIndex(x => x.CasNumber).IsUnique();
            modelBuilder.Entity<IhChemicalSynonym>().HasIndex(x => new { x.IhChemicalId, x.Synonym }).IsUnique();
            modelBuilder.Entity<IhChemicalProperty>().HasIndex(x => new { x.IhChemicalId, x.Key }).IsUnique();
            modelBuilder.Entity<IhChemicalHazard>().HasIndex(x => new { x.IhChemicalId, x.Source, x.Code });
            modelBuilder.Entity<IhChemicalOel>().HasIndex(x => new { x.IhChemicalId, x.Source, x.Type });
            modelBuilder.Entity<IhChemicalSamplingMethod>().HasIndex(x => new { x.IhChemicalId, x.Source, x.MethodId }).IsUnique();

            // --- Performance indexes for foreign key lookups ---
            modelBuilder.Entity<IhChemicalProperty>()
                .HasIndex(x => x.IhChemicalId)
                .HasDatabaseName("ix_ih_chemical_property_ihchemicalid");

            modelBuilder.Entity<IhChemicalSynonym>()
                .HasIndex(x => x.IhChemicalId)
                .HasDatabaseName("ix_ih_chemical_synonym_ihchemicalid");

            modelBuilder.Entity<IhChemicalHazard>()
                .HasIndex(x => x.IhChemicalId)
                .HasDatabaseName("ix_ih_chemical_hazard_ihchemicalid");

            modelBuilder.Entity<IhChemicalOel>()
                .HasIndex(x => x.IhChemicalId)
                .HasDatabaseName("ix_ih_chemical_oel_ihchemicalid");

            modelBuilder.Entity<IhChemicalSamplingMethod>()
                .HasIndex(x => x.IhChemicalId)
                .HasDatabaseName("ix_ih_chemical_sampling_method_ihchemicalid");

            modelBuilder.Entity<IhChemicalSynonym>()
            .HasOne(s => s.IhChemical)
            .WithMany(c => c.Synonyms)
            .HasForeignKey(s => s.IhChemicalId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IhChemicalProperty>()
            .HasOne(s => s.IhChemical)
            .WithMany(c => c.Properties)
            .HasForeignKey(s => s.IhChemicalId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IhChemicalHazard>()
            .HasOne(s => s.IhChemical)
            .WithMany(c => c.Hazards)
            .HasForeignKey(s => s.IhChemicalId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IhChemicalOel>()
            .HasOne(s => s.IhChemical)
            .WithMany(c => c.OELs)
            .HasForeignKey(s => s.IhChemicalId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IhChemicalSamplingMethod>()
            .HasOne(s => s.IhChemical)
            .WithMany(c => c.SamplingMethods)
            .HasForeignKey(s => s.IhChemicalId)
            .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
