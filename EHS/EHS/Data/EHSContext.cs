using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EHS.Models;

namespace EHS.Data
{
    public class EHSContext : DbContext
    {
        public EHSContext (DbContextOptions<EHSContext> options)
            : base(options) {}

        public DbSet<EHS.Models.seg_risk_assessments> seg_risk_assessments { get; set; } = default!;

        public override int SaveChanges()
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimesToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimesToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.ClrType == typeof(DateTime))
                        {
                            var value = (DateTime)property.CurrentValue;
                            if (value.Kind == DateTimeKind.Unspecified)
                            {
                                // Debug/throw here to find the property causing failure
                                throw new InvalidOperationException(
                                    $"Unspecified DateTime found in {entry.Entity.GetType().Name}.{property.Metadata.Name} = {value}");
                            }

                            if (value.Kind == DateTimeKind.Local)
                                property.CurrentValue = value.ToUniversalTime();
                        }

                        if (property.Metadata.ClrType == typeof(DateTime?))
                        {
                            var value = (DateTime?)property.CurrentValue;
                            if (value.HasValue)
                            {
                                if (value.Value.Kind == DateTimeKind.Unspecified)
                                {
                                    throw new InvalidOperationException(
                                        $"Unspecified DateTime? found in {entry.Entity.GetType().Name}.{property.Metadata.Name} = {value.Value}");
                                }

                                if (value.Value.Kind == DateTimeKind.Local)
                                    property.CurrentValue = value.Value.ToUniversalTime();
                            }
                        }
                    }
                }
            }
        }
    }
}
