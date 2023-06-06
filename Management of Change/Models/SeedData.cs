using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Management_of_Change.Data;
using System;
using System.Linq;

namespace Management_of_Change.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new Management_of_ChangeContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<Management_of_ChangeContext>>()))
            {
                // Look for any Change Requests....
                if (!context.ChangeRequest.Any())
                {
                    context.ChangeRequest.AddRange
                    (
                        new ChangeRequest
                        {
                            MOC_Number = "MOC-",
                            Change_Owner = "Michael Wilson",
                            Location_Site = "Location Site 1",
                            Title_Change_Description = "Change in Boule Compound Mixture",
                            Scope_of_the_Change = "Scope of the Change 1",
                            Justification_of_the_Change = "Justification of Change 1",
                            Change_Status = "Change Status 1",
                            Request_Date = System.DateTime.Now,
                            Proudct_Line = "Product Line 1",
                            Change_Type = "Change Type 1",
                            Estimated_Completion_Date = System.DateTime.Now.AddYears(1),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 1",
                            Change_Level = "Change Level 1",
                            Area_of_Change = "Area of Change 1",
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(3)
                        },

                        new ChangeRequest
                        {
                            MOC_Number = "MOC-",
                            Change_Owner = "Joe Jackson",
                            Location_Site = "Location Site 2",
                            Title_Change_Description = "Furnace Door Change",
                            Scope_of_the_Change = "Scope of the Change 2",
                            Justification_of_the_Change = "Justification of Change 2",
                            Change_Status = "Change Status 2",
                            Request_Date = System.DateTime.Now.AddYears(-2),
                            Proudct_Line = "Product Line 2",
                            Change_Type = "Change Type 2",
                            Estimated_Completion_Date = System.DateTime.Now.AddYears(2),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 2",
                            Change_Level = "Change Level 2",
                            Area_of_Change = "Area of Change 2",
                            Expiration_Date_Temporary = null
                        },

                        new ChangeRequest
                        {
                            MOC_Number = "MOC-",
                            Change_Owner = "Test Testerson",
                            Location_Site = "Location Site 3",
                            Title_Change_Description = "Organization Chart Change",
                            Scope_of_the_Change = "Scope of the Change 3",
                            Justification_of_the_Change = "Justification of Change 3",
                            Change_Status = "Change Status 3",
                            Request_Date = System.DateTime.Now.AddMonths(-2),
                            Proudct_Line = "Product Line 3",
                            Change_Type = "Change Type 3",
                            Estimated_Completion_Date = System.DateTime.Now.AddMonths(1),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 3",
                            Change_Level = "Change Level 3",
                            Area_of_Change = "Area of Change 3",
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(7)
                        }
                    );
                    context.SaveChanges();
                }
                

                // Look for any Change Types.....
                if (!context.ChangeType.Any())
                {
                    context.ChangeType.AddRange(
                        new ChangeType
                        {
                            Type = "EHS System",
                            Description = "Implement requirements of the EHS ISO14001/45001 Management System.  Typically the result of regulatory standard updates.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Equipment Installation",
                            Description = "-Specifying new equipment for purchase and installation. -Modification of existing equipment that is not break/fix maintenance or like-for-like replacement. -Moving existing equipment to a new location. NOTE: All equipment changes should have MOC initiated prior to purchase or as early as possible to ensure all reviews are completed. Consider using the optional New Equipment Checklist (last tab in the template).",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Equipment Qualification",
                            Description = "'Qualification or requalification of equipment (requires CMT approval). NOTE: A separate MOC is required for Equipment Installation, see Equipment Change Type above",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Equipment Spec",
                            Description = "Any change to existing purchased equipment that should result in an internal equipment specification change to ensure all future equipment is properly specified (i.e. sight glass material, hardware or electrical changes, furnace specifications, piping specifications, drawing dimensions for components).",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Facility Infrastructure",
                            Description = "Facility infrastructure changes such as a new compressed air system, etc.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "IT",
                            Description = "Changes to IT systems that impact personnel or have the potential to impact final product.  Includes new software or hardware used in production process equipment or testing equipment.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Labeling",
                            Description = "Changes to product or shipping labels",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Production Process",
                            Description = "-Process parameter changes which are outside of approved limits. -Process route or step changes which are outside of currently approved process flow. -Includes control systems changes. Note: Impact assessments are different based on the change Level (1-3 vs 4-5) and potential to impact the final product.  Recipe changes are managed through a different change type.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Process Safety",
                            Description = "Changes to process safety devices (e.g. gas detection sensors, alarms, etc.). NOTE: If new equipment, use Equipment Change Type which has Process Safety review.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Product",
                            Description = "-Changes to the specification requirements of the standard CSS specification. -Implementation of new products",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Quality System",
                            Description = "Implement requirements of the ISO9001 Quality Management System",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Raw Material",
                            Description = "-Raw material specification changes. -New raw materials. -Change to raw material supplier (new supplier, new name)",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Recipe",
                            Description = "Recipe changes that are intended to produce saleable product and modify parameters outside of approved operating conditions.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = "Mean Joe",
                            DeletedDate = DateTime.Now.AddYears(-1)
                        },
                        new ChangeType
                        {
                            Type = "Supplied Material\r\n",
                            Description = "Change in any supplied (MRO) material",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Fred Jones",
                            ModifiedDate = DateTime.Now.AddMonths(-3),
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Change Levels.....
                if (!context.ChangeLevel.Any())
                {
                    context.ChangeLevel.AddRange(
                        new ChangeLevel
                        {
                            Level = "Level 1 - Major",
                            Description = "Any change that affects form, fit, or function of the product which would result in redefining the production specification. <b>Impacted customers are notified and must provide written approval of the change.</b>",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeLevel
                        {
                            Level = "Level 2 - Major",
                            Description = "Any change that affects form, fit, or function of the product but does not require to redefine the product specification.  Impacted customers may be notified but no formal approval of the change is required.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeLevel
                        {
                            Level = "Level 3 - Minor",
                            Description = "A minor change with no customer impact therefore requiring no customer notification.",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeLevel
                        {
                            Level = "Level 4 - Minor",
                            Description = "Change to process or systems which has no effect on the product (e.g. changing an O2 sensor, process support equipment, upgrading software, minor recipe adjustment, etc.)",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = "Christie Tester",
                            DeletedDate = DateTime.Now.AddYears(-2)
                        },
                        new ChangeLevel
                        {
                            Level = "Level 5 - Temporary",
                            Description = "Used to document a temporary change to the process (e.g. R&D trials, extended engineering trials, etc.)",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "David Hasselhoff",
                            ModifiedDate = DateTime.Now.AddDays(-5),
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

            }
        }
    }
}
