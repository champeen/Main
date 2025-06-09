using EHS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EHS.Data;
using System;
using System.Linq;
using EHS.Utilities;
using System.Threading.Tasks;

namespace EHS.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EHSContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<EHSContext>>()))
            {
                // Look for any Change Requests....
                if (!context.seg_risk_assessments.Any())
                {
                    for (int i = 0; i < 22; i++)
                    {
                        context.seg_risk_assessments.AddRange(
                            new seg_risk_assessments
                            {
                                location = i.ToString(),
                                exposure_type = i.ToString(),
                                agent = i.ToString(),
                                seg_role = i.ToString(),
                                task = i.ToString(),
                                oel = i.ToString(),
                                acute_chronic = i.ToString(),
                                route_of_entry = i.ToString(),
                                frequency_of_task = i.ToString(),
                                duration_of_task = i.ToString(),
                                monitoring_data_required = i.ToString(),
                                controls_recommended = i.ToString(),
                                exposure_levels_acceptable = i.ToString(),
                                date_conducted = DateTime.Now,
                                assessment_methods_used = i.ToString(),
                                seg_number_of_workers = i,
                                has_agent_been_changed = i.ToString(),
                                person_performing_assessment = i.ToString(),
                                created_user = "MJWilson",
                                created_date = DateTime.Now
                            }
                        );
                        context.SaveChanges();
                    };                   
                }
                //// Look for any Change Types.....
                //if (!context.ChangeType.Any())
                //{
                //    context.ChangeType.AddRange(
                //        new ChangeType
                //        {
                //            Type = "EHS System",
                //            Description = "Implement requirements of the EHS ISO14001/45001 Management System.  Typically the result of regulatory standard updates.",
                //            Order = "05",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Equipment Installation",
                //            Description = "-Specifying new equipment for purchase and installation. -Modification of existing equipment that is not break/fix maintenance or like-for-like replacement. -Moving existing equipment to a new location. NOTE: All equipment changes should have MOC initiated prior to purchase or as early as possible to ensure all reviews are completed. Consider using the optional New Equipment Checklist (last tab in the template).",
                //            Order = "10",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Equipment Qualification",
                //            Description = "'Qualification or requalification of equipment (requires CMT approval). NOTE: A separate MOC is required for Equipment Installation, see Equipment Change Type above",
                //            Order = "15",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Equipment Spec",
                //            Description = "Any change to existing purchased equipment that should result in an internal equipment specification change to ensure all future equipment is properly specified (i.e. sight glass material, hardware or electrical changes, furnace specifications, piping specifications, drawing dimensions for components).",
                //            Order = "20",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Facility Infrastructure",
                //            Description = "Facility infrastructure changes such as a new compressed air system, etc.",
                //            Order = "25",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "IT",
                //            Description = "Changes to IT systems that impact personnel or have the potential to impact final product.  Includes new software or hardware used in production process equipment or testing equipment.",
                //            Order = "30",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Labeling",
                //            Description = "Changes to product or shipping labels",
                //            Order = "35",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Production Process Level 1-3",
                //            Description = "-Process parameter changes which are outside of approved limits. -Process route or step changes which are outside of currently approved process flow. -Includes control systems changes. Note: Impact assessments are different based on the change Level (1-3 vs 4-5) and potential to impact the final product.  Recipe changes are managed through a different change type.",
                //            Order = "40",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Production Process Level 4-5",
                //            Description = "-Process parameter changes which are outside of approved limits. -Process route or step changes which are outside of currently approved process flow. -Includes control systems changes. Note: Impact assessments are different based on the change Level (1-3 vs 4-5) and potential to impact the final product.  Recipe changes are managed through a different change type.",
                //            Order = "45",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Process Safety",
                //            Description = "Changes to process safety devices (e.g. gas detection sensors, alarms, etc.). NOTE: If new equipment, use Equipment Change Type which has Process Safety review.",
                //            Order = "50",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Product",
                //            Description = "-Changes to the specification requirements of the standard CSS specification. -Implementation of new products",
                //            Order = "55",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Quality System",
                //            Description = "Implement requirements of the ISO9001 Quality Management System",
                //            Order = "60",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Raw Material",
                //            Description = "-Raw material specification changes. -New raw materials. -Change to raw material supplier (new supplier, new name)",
                //            Order = "65",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Recipe",
                //            Description = "Recipe changes that are intended to produce saleable product and modify parameters outside of approved operating conditions.",
                //            Order = "70",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        },
                //        new ChangeType
                //        {
                //            Type = "Supplied Material",
                //            Description = "Change in any supplied (MRO) material",
                //            Order = "80",
                //            CreatedUser = "MJWilson",
                //            CreatedDate = DateTime.Now,
                //            ModifiedUser = null,
                //            ModifiedDate = null,
                //            DeletedUser = null,
                //            DeletedDate = null
                //        }
                //    );
                //    context.SaveChanges();
                //}       
            }
        }
    }
}
