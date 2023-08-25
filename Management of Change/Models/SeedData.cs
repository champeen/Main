using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Management_of_Change.Data;
using System;
using System.Linq;
using Management_of_Change.Utilities;

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
                            MOC_Number = "MOC-230707-1",
                            Change_Owner = "MJWilson",
                            Location_Site = "Location Site 1",
                            Title_Change_Description = "Change in Boule Compound Mixture",
                            Scope_of_the_Change = "Scope of the Change 1",
                            Justification_of_the_Change = "Justification of Change 1",
                            Change_Status = "Draft",
                            Request_Date = System.DateTime.Now,
                            Proudct_Line = "Product Line 1",
                            Change_Type = "Change Type 1",
                            Estimated_Completion_Date = System.DateTime.Now.AddYears(1),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 1",
                            Change_Level = "Change Level 1",
                            Area_of_Change = "Area of Change 1",
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(3),
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },

                        new ChangeRequest
                        {
                            MOC_Number = "MOC-230707-2",
                            Change_Owner = "Joe Jackson",
                            Location_Site = "Location Site 2",
                            Title_Change_Description = "Furnace Door Change",
                            Scope_of_the_Change = "Scope of the Change 2",
                            Justification_of_the_Change = "Justification of Change 2",
                            Change_Status = "Killed",
                            Request_Date = System.DateTime.Now.AddYears(-2),
                            Proudct_Line = "Product Line 2",
                            Change_Type = "Change Type 2",
                            Estimated_Completion_Date = System.DateTime.Now.AddYears(2),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 2",
                            Change_Level = "Change Level 2",
                            Area_of_Change = "Area of Change 2",
                            Expiration_Date_Temporary = null,
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Fred Jones",
                            ModifiedDate = DateTime.Now.AddMonths(-3),
                            DeletedUser = "Jesse Girl",
                            DeletedDate = DateTime.Now.AddDays(-2)
                        },

                        new ChangeRequest
                        {
                            MOC_Number = "MOC-230708-1",
                            Change_Owner = "Test Testerson",
                            Location_Site = "Location Site 3",
                            Title_Change_Description = "Organization Chart Change",
                            Scope_of_the_Change = "Scope of the Change 3",
                            Justification_of_the_Change = "Justification of Change 3",
                            Change_Status = "Draft",
                            Request_Date = System.DateTime.Now.AddMonths(-2),
                            Proudct_Line = "Product Line 3",
                            Change_Type = "Change Type 3",
                            Estimated_Completion_Date = System.DateTime.Now.AddMonths(1),
                            Raw_Material_Component_Numbers_Impacted = "Raw Materials Component Numbers Impacted 3",
                            Change_Level = "Change Level 3",
                            Area_of_Change = "Area of Change 3",
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(7),
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
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
                            Order = "05",
                            CreatedUser = "MJWilson",
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
                            Order = "10",
                            CreatedUser = "MJWilson",
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
                            Order = "15",
                            CreatedUser = "MJWilson",
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
                            Order = "20",
                            CreatedUser = "MJWilson",
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
                            Order = "25",
                            CreatedUser = "MJWilson",
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
                            Order = "30",
                            CreatedUser = "MJWilson",
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
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Production Process Level 1-3",
                            Description = "-Process parameter changes which are outside of approved limits. -Process route or step changes which are outside of currently approved process flow. -Includes control systems changes. Note: Impact assessments are different based on the change Level (1-3 vs 4-5) and potential to impact the final product.  Recipe changes are managed through a different change type.",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeType
                        {
                            Type = "Production Process Level 4-5",
                            Description = "-Process parameter changes which are outside of approved limits. -Process route or step changes which are outside of currently approved process flow. -Includes control systems changes. Note: Impact assessments are different based on the change Level (1-3 vs 4-5) and potential to impact the final product.  Recipe changes are managed through a different change type.",
                            Order = "45",
                            CreatedUser = "MJWilson",
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
                            Order = "50",
                            CreatedUser = "MJWilson",
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
                            Order = "55",
                            CreatedUser = "MJWilson",
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
                            Order = "60",
                            CreatedUser = "MJWilson",
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
                            Order = "65",
                            CreatedUser = "MJWilson",
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
                            Order = "70",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = "Mean Joe",
                            DeletedDate = DateTime.Now.AddYears(-1)
                        },
                        new ChangeType
                        {
                            Type = "RND Recipe Transfer",
                            Description = "?",
                            Order = "75",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = "Mean Joe",
                            DeletedDate = DateTime.Now.AddYears(-1)
                        },
                        new ChangeType
                        {
                            Type = "Supplied Material",
                            Description = "Change in any supplied (MRO) material",
                            Order = "80",
                            CreatedUser = "MJWilson",
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
                            Order = "05",
                            CreatedUser = "MJWilson",
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
                            Order = "10",
                            CreatedUser = "MJWilson",
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
                            Order = "15",
                            CreatedUser = "MJWilson",
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
                            Order = "20",
                            CreatedUser = "MJWilson",
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
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "David Hasselhoff",
                            ModifiedDate = DateTime.Now.AddDays(-5),
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Change Status.....
                if (!context.ChangeStatus.Any())
                {
                    context.ChangeStatus.AddRange(
                        new ChangeStatus
                        {
                            Status = "Draft",
                            Default = true,
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Submitted for Impact Assessment Review",
                            Default = false,
                            Order = "10",
                            CreatedUser = "Joe Jackson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Awaiting Completion of Pre Implementation Tasks",
                            Default = false,
                            Order = "15",
                            CreatedUser = "Steve Smith",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Submitted for Final Approvals",
                            Default = false,
                            Order = "20",
                            CreatedUser = "Fred Bear",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Submitted for Implementation",
                            Default = false,
                            Order = "25",
                            CreatedUser = "Ken Jones",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Closed",
                            Default = false,
                            Order = "30",
                            CreatedUser = "Shelly Shelby",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "On Hold",
                            Default = false,
                            Order = "35",
                            CreatedUser = "Johanna Jones",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Killed",
                            Default = false,
                            Order = "40",
                            CreatedUser = "Johanna Jones",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Change Status.....
                if (!context.ResponseDropdownSelections.Any())
                {
                    context.ResponseDropdownSelections.AddRange(
                        new ResponseDropdownSelections
                        {
                            Response = "Yes",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ResponseDropdownSelections
                        {
                            Response = "No",
                            Order = "10",
                            CreatedUser = "Joe Jackson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ResponseDropdownSelections
                        {
                            Response = "N/A",
                            Order = "15",
                            CreatedUser = "Steve Smith",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Randy Radar",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Lisa Matthews",
                            DeletedDate = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Product Lines.....
                if (!context.ProductLine.Any())
                {
                    context.ProductLine.AddRange(
                        new ProductLine
                        {
                            Description = "All Products",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "100mm bare",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "150mm bare",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "200mm bare",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "All Epi",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "100mm Epi",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "150mm Epi",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "200mm Epi",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "165mm seed",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "Multiple",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Site/Locations.....
                if (!context.SiteLocation.Any())
                {
                    context.SiteLocation.AddRange(
                        new SiteLocation
                        {
                            Description = "All",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new SiteLocation
                        {
                            Description = "Auburn",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new SiteLocation
                        {
                            Description = "Bay City",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Joe Jackson",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Jackson Browne",
                            DeletedDate = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any ChangeAreas.....
                if (!context.ChangeArea.Any())
                {
                    context.ChangeArea.AddRange(
                        new ChangeArea
                        {
                            Description = "Growth",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Fab",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Grind",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Joe Jackson",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Jackson Browne",
                            DeletedDate = DateTime.Now
                        },
                        new ChangeArea
                        {
                            Description = "Polish",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Clean & Metrology",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Epi",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Sort & Package",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Disposition",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Other",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any GeneralMocQuestions.....
                if (!context.GeneralMocQuestions.Any())
                {
                    context.GeneralMocQuestions.AddRange(
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require new SOPs, Runsheets, Checklists, etc? If yes, ensure they are included in the MOC folder and upon MOC approval are submitted through the proper Document Management System process for review.",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require the update of existing approved SOPs, Runsheets, Checklists, etc?  If yes, ensure they are included in the MOC folder and upon MOC approval are submitted through the proper Document Management System process for review. ",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require temporary SOPs, runsheets or checklists?  If yes, ensure they are included in the MOC folder and upon MOC approval are submitted through the proper Document Management System process for review. ",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require training?  If yes, ensure that the scope and justification of the training is provided in the Training Checklist and Impact Assessment is checked as required below.",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change involve new chemicals or an increase in chemical quantities for the Site?  If yes, ensure that Environment and Health and Industrial Hygiene Impact Assessments are checked as required below.",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require IT support for new hardware, software, increased data storage, or a update/add to MES?  If yes, ensure IT checklist is checked as required below.",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require a new raw, intermediate, or Finished Goods material number?  If yes, ensure that the responsible Engineer has a task to begin the new material creation process in the Supply Chain Impact Assessment Checklist.\t\t\t\t\t\t\r\n",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require additional WIP packaging components?  If yes, provide information on how many in the Supply Chain Impact Assessment Checklist.",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change impact existing raw material usage quantities (increase or decrease)?  If yes, provide detailed quantities in the Supply Chain Impact Assessment Checklist.",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Is this change associated with a PTN?  If yes, please enter the PTN number(s) to the right.",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require the use of a waiver for the release of final product?  If so, please enter the waiver number(s) to the right.",
                            Order = "55",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Is this change associated with a CMT (NOTE: all Level 1-3 changes require CMT)? If so, enter CMT number to the right and/or initiate CMT with CMT Leader.",
                            Order = "60",
                            CreatedUser = "Joe",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Ron",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Fred",
                            DeletedDate = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any ReviewTypes.....
                if (!context.ReviewType.Any())
                {
                    context.ReviewType.AddRange(
                        new ReviewType
                        {
                            Type = "Device Expert",
                            Reviewer = "Gil Chung",
                            Email = "gil.chung@sksiltron.com",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Environmental",
                            Reviewer = "Danny Bennett",
                            Email = "Danny.Bennett@sksiltron.com",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Equipment",
                            Reviewer = "Equipment Reviewer Person",
                            Email = "Equipment_Reviewer@sksiltron.com",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Joe Jackson",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Jackson Browne",
                            DeletedDate = DateTime.Now
                        },
                        new ReviewType
                        {
                            Type = "Health & IH",
                            Reviewer = "Tammy Dubey",
                            Email = "Tammy.Dubey@sksiltron.com",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "HR",
                            Reviewer = "Tiffany Kukla",
                            Email = "Tiffany.Kukla@sksiltron.com",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "IT",
                            Reviewer = "Nagesh Sampangi",
                            Email = "Nagesh.Sampangi@sksiltron.com",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Maintenance & Reliability",
                            Reviewer = "EquipmentOwner",
                            Email = "EquipmentOwner@sksiltron.com",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Operations - General",
                            Reviewer = "Brandon Rohde",
                            Email = "Brandon.Rohde@sksiltron.com",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Operations - EPI",
                            Reviewer = "Jim Wu",
                            Email = "Jim.Wu@sksiltron.com",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Personal Safety",
                            Reviewer = "Jason Welke",
                            Email = "Jason.Welke@sksiltron.com",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Process Control & Engineering - Growth Equipment",
                            Reviewer = "Alex Kim",
                            Email = "Alex.Kim@sksiltron.com",
                            Order = "55",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Process Control & Engineering - Growth Process",
                            Reviewer = "Sungchul Baek",
                            Email = "Sungchul.Baek@sksiltron.com",
                            Order = "60",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Process Control & Engineering - Wafering",
                            Reviewer = "Sungpyo Jung",
                            Email = "Sungpyo.Jung@sksiltron.com",
                            Order = "65",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Process Control & Engineering - Controls",
                            Reviewer = "Jeff Wood",
                            Email = "Jeff.Wood@sksiltron.com",
                            Order = "70",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Process Safety",
                            Reviewer = "Ryan Ralph",
                            Email = "Ryan.Ralph@sksiltron.com",
                            Order = "75",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Production Planning",
                            Reviewer = "Stacey Goss",
                            Email = "Stacey.Goss@sksiltron.com",
                            Order = "80",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Quality",
                            Reviewer = "Marvin Lee",
                            Email = "Marvin.Lee@sksiltron.com",
                            Order = "85",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Research & Development",
                            Reviewer = "Ian Manning",
                            Email = "ian.manning@sksiltron.com",
                            Order = "90",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Supply Chain",
                            Reviewer = "Howard McCready",
                            Email = "howard.mccready@sksiltron.com",
                            Order = "95",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ReviewType
                        {
                            Type = "Training",
                            Reviewer = "Jennifer Gooch",
                            Email = "Jennifer.Gooch@sksiltron.com",
                            Order = "98",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any FinalReviewTypes.....
                if (!context.FinalReviewType.Any())
                {
                    context.FinalReviewType.AddRange(
                        new FinalReviewType
                        {
                            Type = "EH&S Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "Quality Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "Operations Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Joe Jackson",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Jackson Browne",
                            DeletedDate = DateTime.Now
                        },
                        new FinalReviewType
                        {
                            Type = "Engineering Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "Facilities Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "IT Manager",
                            Reviewer = "Nagesh Sampangi",
                            Email = "Nagesh.Sampangi@sksiltron.com",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "Supply Chain Manager",
                            Reviewer = "?",
                            Email = "?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new FinalReviewType
                        {
                            Type = "Commercial Manager",
                            Reviewer = "Chris Toelle",
                            Email = "Chris.Toelle@sksiltron.com",
                            Order = "3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Impact Assessment Matrix Records.....
                if (!context.ImpactAssessmentMatrix.Any())
                {
                    context.ImpactAssessmentMatrix.AddRange(
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Supplied Material\t",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Environmental",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Equipment",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Health & IH",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Labeling",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "IT",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Maintenance & Reliability",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Maintenance & Reliability",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Maintenance & Reliability",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Maintenance & Reliability",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Maintenance & Reliability",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - General",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Operations - EPI",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Personal Safety",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                       new ImpactAssessmentMatrix
                       {
                           ReviewType = "Process Control & Engineering - Growth Process",
                           ChangeType = "Equipment Installation",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                       new ImpactAssessmentMatrix
                       {
                           ReviewType = "Process Control & Engineering - Controls",
                           ChangeType = "Equipment Installation",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                       new ImpactAssessmentMatrix
                       {
                           ReviewType = "Process Safety",
                           ChangeType = "EHS System",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImpactAssessmentMatrix
                       {
                           ReviewType = "Process Safety",
                           ChangeType = "Equipment Installation",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Process Safety",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Production Planning",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Labeling",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Quality",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "RND Recipe Transfer",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Research & Development",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Labeling",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Product\t",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Supply Chain",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "IT",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Labeling",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Training",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }
                // Look for any Impact Assessment Matrix Records.....
                if (!context.ImplementationFinalApprovalMatrix.Any())
                {
                    context.ImplementationFinalApprovalMatrix.AddRange(
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Commercial Manager",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "EHS System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "EH&S Manager",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Labeling",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Quality System",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Quality Manager",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Operations Manager",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Equipment Qualification",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Facility Infrastructure",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Production Process Level 1-3",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Production Process Level 4-5",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Process Safety",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Product",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Raw Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Recipe",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Supplied Material\t",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Engineering Manager",
                            ChangeType = "Supplied Material",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Facilities Manager",
                            ChangeType = "",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Facilities Manager",
                           ChangeType = "Equipment Installation",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                        new ImplementationFinalApprovalMatrix
                        {
                            FinalReviewType = "Facilities Manager",
                            ChangeType = "Equipment Spec",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Facilities Manager",
                           ChangeType = "Facility Infrastructure",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "IT Manager",
                           ChangeType = "IT",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Supply Chain Manager",
                           ChangeType = "Labeling",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Supply Chain Manager",
                           ChangeType = "Production Process Level 1-3",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Supply Chain Manager",
                           ChangeType = "Production Process Level 4-5",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       },
                       new ImplementationFinalApprovalMatrix
                       {
                           FinalReviewType = "Supply Chain Manager",
                           ChangeType = "Supplied Material",
                           CreatedUser = "MJWilson",
                           CreatedDate = DateTime.Now,
                           ModifiedUser = null,
                           ModifiedDate = null,
                           DeletedUser = null,
                           DeletedDate = null
                       }
                    );
                    context.SaveChanges();
                }

                // Look for any Impact Assessment Response Questions.....
                if (!context.ImpactAssessmentResponseQuestions.Any())
                {
                    context.ImpactAssessmentResponseQuestions.AddRange(
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Device Expert",
                            Question = "Does the change have the potential to impact device manufacturing?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Device Expert",
                            Question = "Will wafers need to be tested to determine impact to device manufacturing?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Device Expert",
                            Question = "Does the change require customer notification due to impact to device manufacturing?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Device Expert",
                            Question = "Is additional data needed before the change can be approved?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does the change involve excavation or demolition? ",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Are there legal reviews and documents required concerning the change?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does the change require the aspects and impacts risk register to be reviewed and updated? (If not ISO14001 aspect then N/A)",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Will this change effect Environmental compliance?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Are there any chemicals or materials involved in this change (new chemicals, quantity change)?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does the change impact a chemical with a low odor threshold?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does this change require a modification to the appropriate and/or required labeling (content reactivity, flammability, etc.) of the vessels, lines, or containers?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Is there any waste or wastewater stream involved in this change?",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does this change impact the environment in any way (air, land, water)?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Are you making any changes to the EH&S requirements or making any changes that would require external permits or approvals?",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does the change impact an existing, or require a new, Michigan Department of Environmental Quality (MDEQ) flammable/combustible liquid storage tank registration?",
                            Order = "55",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Is there an impact to the warehouse storage area allowed quantities? (including quantity package size or type, or method of shipping for raw materials, products, or waste)?",
                            Order = "60",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Has the startup/shutdown procedure and checklist been completed?",
                            Order = "65",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does this change require a new task or change an existing task in MES?",
                            Order = "70",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Are any infrastructure changes needed for Environmental compliance? (Signage, waste handling, vents, etc.)?",
                            Order = "75",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Environmental",
                            Question = "Does this change involve the disposal of contaminated piping, metal or equipment?",
                            Order = "80",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Does the change involve addition, deletion or modification of equipment or instrumentation?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "If the change is for new equipment, review and complete the 'New Equipment Checklist' as necessary",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Is a maintenance and reliability review needed for the change (Access, PMs, Critical spare parts, etc.)?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Has material of construction compatibility with the process been considered?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Does the new equipment require any special permitting/license (X-ray, Nuclear, boiler, etc.)?  Please see EHS Start-up/Shut-down Procedure.",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Is a pre-inspection required at the Vendor for this equipment?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Are there legal reviews and/or document required concerning the change?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Does this change involve a process safety device?  If yes, ensure Process Safety review is included in the impact assessment.",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Does this change require Pre-Startup Safety Review (PSSR)? If so, ensure completion",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Equipment",
                            Question = "Do P&IDs need to be created/updated for equipment addition or change?  Assign task to appropriate Engineer.",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Does this change involve or disturb regulated chemicals (OSHA, EPA, etc.)?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Will this change impact any Industrial Hygiene programs?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Are any HazCom changes required? (SDS, training, PPE, etc.)",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Are any IH risk assessments required for this change? (Respiratory, ventilation, PPE, Noise, etc.)",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Will this change impact any laser or radiation programs?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Does this change require a new task or change an existing task in MES?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Does this change involve the release or disposal or scrap metal or equipment?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Are there any chemicals or materials involved in this change? ",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Does the change impact a chemical with a low odor threshold?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Health & IH",
                            Question = "Is an ergonomic evaluation required for this change?",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "Will any HR resources be required to complete the change?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "What impact does the change have on employees?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "If any, what impact does this change have on labor/employment laws?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "What is the required timing of the change?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "What impact does this change have on headcount?  Increase/Decrease?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "What impact does the change have on employment practices and policies? ",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "Do new policies need to be created?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "Are there on-boarding/off-boarding required for this change?",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "HR",
                            Question = "Are there legal reviews and documents required concerning the change?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Will any IT resources be required to complete the change?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does this change involve any IT hardware or equipment?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Is there a change in security requirements?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does additional software need to be procured?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does additional hardware need to be procured?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does this require data storage, if so how much and for how long does the data need to be retained?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does this require an MES route change or new route?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does this require a new product in MES?",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "IT",
                            Question = "Does data from this need to be added to MES?  If so - as an EDC or EI?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Does containment need to be considered for this change, such as for deluge water, rainwater runoff, etc.?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Have the process or design boundary conditions changed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Where applicable, have all engineering disciplines been involved with the design or review of this change?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Does this change involve any equipment or instrumentation, such as pumps or other rotating equipment, vessels, piping, any instrumentation,  IT hardware, vehicles, etc.?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Does this change involve or impact personnel safety or an area where personnel work, including protection against exposure or injury?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Should consideration be given to performing equipment maintenance (e.g. is a maintenance SOP needed or  MES PM tasks)?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Maintenance & Reliability",
                            Question = "Do PMs need to be setup in MES?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Does this change require updating existing SOPs, Runsheets, or other Records?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Does this change require new SOPs, Runsheets, or other records?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Documentation Note:  If a temporary SOP/runsheet is needed it should have an expiration date.  Review all attachments in the MOC folder and assign actions as needed to address updates.  Operations should ensure an approved SOP/Runsheet/etc. is in place before the temporary expires.",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Does the location or ease of use for valves, switches, or any operating device impact operability or functionality?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Will this change impact the physical environment (e.g. heating and cooling change) in which the operator is working?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - General",
                            Question = "Does this change impact the material work flow such that LEAN manufacturing practices should be considered?  If new equipment, how will this flow/function?  Consider operator movement, inventory management, flow of the product through the process.",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Does this change require updating existing SOPs, Runsheets, or other Records?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Does this change require new SOPs, Runsheets, or other records?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Documentation Note:  If a temporary SOP/runsheet is needed it should have an expiration date.  Review all attachments in the MOC folder and assign actions as needed to address updates.  Operations should ensure an approved SOP/Runsheet/etc. is in place before the temporary expires.",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Does the location or ease of use for valves, switches, or any operating device impact operability or functionality?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Will this change impact the physical environment (e.g. heating and cooling change) in which the operator is working?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Operations - EPI",
                            Question = "Does this change impact the material work flow such that LEAN manufacturing practices should be considered?  If new equipment, how will this flow/function?  Consider operator movement, inventory management, flow of the product through the process.",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Does this change require a modification in the appropriate and/or require labeling (content, reactivity, flammability, etc.) of the vessels, lines, or containers? ",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Does this change involve or disturb regulated chemicals (Gov’t Agencies)?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Does this change involve or impact personnel safety or an area where personnel work, including protection against exposure or injury? (Including Industrial Hygiene)",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Is any type of protective system impacted or needed for this change? (e.g. emergency response, security, alarms, fire protection, or interlocks)",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Are you making any changes to the EH&S requirements or making any changes that would require external permits or approvals?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Has area near the change been reviewed for personnel safety?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Does this change require a new task or change an existing task in MES?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Has the startup/shutdown procedure and checklist been completed?",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = " Are any infrastructure changes needed for safety compliance? (Signage, fire ext., safety showers, etc.)",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Personal Safety",
                            Question = "Does ES&S need to review the impact on security (e.g. exterior doors) for this change? - Safety",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Should the control strategy be evaluated for any control points changing or best practices that should be followed?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Is there a plant specific checklist or procedure for the code changes that should be followed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Should the control logic be simulated or have additional reviews?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Is the Voice Messaging System going to be impacted due to this change?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Does new instrumentation or modifications to existing instrumentation need to be considered for process change?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Will new process control hardware be needed for this change?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Does this change involve a chemical manufacturing process?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Is any type of protective system impacted or needed for this change? (e.g. emergency response, security, alarms, fire protection, or interlocks)",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Equipment",
                            Question = "Does this change require Dow approval?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Should the control strategy be evaluated for any control points changing or best practices that should be followed?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Is there a plant specific checklist or procedure for the code changes that should be followed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Should the control logic be simulated or have additional reviews?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Is the Voice Messaging System going to be impacted due to this change?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Does new instrumentation or modifications to existing instrumentation need to be considered for process change?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Will new process control hardware be needed for this change?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Does this change involve a chemical manufacturing process?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Is any type of protective system impacted or needed for this change? (e.g. emergency response, security, alarms, fire protection, or interlocks)",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Growth Process",
                            Question = "Does this change require Dow approval?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Should the control strategy be evaluated for any control points changing or best practices that should be followed?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Is there a plant specific checklist or procedure for the code changes that should be followed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Should the control logic be simulated or have additional reviews?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Is the Voice Messaging System going to be impacted due to this change?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Does new instrumentation or modifications to existing instrumentation need to be considered for process change?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Will new process control hardware be needed for this change?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Does this change involve a chemical manufacturing process?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Is any type of protective system impacted or needed for this change? (e.g. emergency response, security, alarms, fire protection, or interlocks)",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Wafering",
                            Question = "Does this change require Dow approval?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Should the control strategy be evaluated for any control points changing or best practices that should be followed?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Is there a plant specific checklist or procedure for the code changes that should be followed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Should the control logic be simulated or have additional reviews?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Is the Voice Messaging System going to be impacted due to this change?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Does new instrumentation or modifications to existing instrumentation need to be considered for process change?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Will new process control hardware be needed for this change?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Does this change involve a chemical manufacturing process?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Is any type of protective system impacted or needed for this change? (e.g. emergency response, security, alarms, fire protection, or interlocks)",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Control & Engineering - Controls",
                            Question = "Does this change require Dow approval?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does this change involve or disturb regulated chemicals (OSHA, EPA,etc.)?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Should the off-site impact be reviewed?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Do procedures providing lines of defense or protection layers against process safety accidents need to be evaluated to determine if they should be designated Critical or Emergency?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does compatibility of waste streams need to be evaluated?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Are there any chemical or materials involved in this change?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does chemical compatibility need to be evaluated?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does this change involve any equipment or instrumentation, such as pump or other rotating equipment, vessels, piping and instrumentations, IT hardware, vehicles, etc.?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does this change involve or impact any infrastructures, such as buildings, structures, utilities, etc.?",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does this change involve any type of technology, engineering discipline, specifications, or best practices?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Is any type pf protective system impacted or needed for this change? (emergency response, security alarms, fire protection, or interlocks)",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Process Safety",
                            Question = "Does this change require inerting, bonding, grounding evaluation?",
                            Order = "55",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Production Planning",
                            Question = "Does this change require any WIP or finished goods to be put on hold or scrapped?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Production Planning",
                            Question = "Does this change require a new Intermediate or Finished Good material number?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Production Planning",
                            Question = "Does this change effect the cycle time or capacity of a tool long term?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Production Planning",
                            Question = "Will this change require any downtime?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require an update to Quality procedures?  Any procedures, runsheets or checklists that may need updating?  Review/approve updated procedures that are included in the MOC folder.",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require customer notification?  PCN must be completed prior to change implementation.",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require specifications (raw material and/or product) to be updated?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require data package(s) to be updated?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require SPC limits to be reviewed?",
                            Order = "25",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require Process capability to be reviewed?",
                            Order = "30",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require Gage R&R/MSA to be reviewed?",
                            Order = "35",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require the Control Plan and pFMEA to be reviewed? (Note: control plans are not available for each step but eventually will be per IATF requirements)",
                            Order = "40",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require the Risk Register to be reviewed?",
                            Order = "45",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change affect the approved supplier list?  If new supplier approval OR new material from an existing approved supplier, ensure that upon completion of Implementation, the supplier notification is sent. (POST-IMPLEMENTATION action)",
                            Order = "50",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change involve any in process specifications and/or testing?",
                            Order = "55",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Quality",
                            Question = "Does this change require an update to the disposition procedures?",
                            Order = "60",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Research & Development",
                            Question = "Does the proposed change involve a material or process that has not been previously used?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Research & Development",
                            Question = "Does the proposed change impact a material that will be received by a customer?",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Research & Development",
                            Question = "Will the change make an impact that the customer will be capable of observing?",
                            Order = "15",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Research & Development",
                            Question = "Does this change negatively impact quality or metrology?",
                            Order = "20",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Supply Chain",
                            Question = "Does this change require a new raw material or change to an existing material?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Supply Chain",
                            Question = "Does this change require a new supplier?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Supply Chain",
                            Question = "Does this change require new WIP packaging (boats/cassettes, etc)?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Supply Chain",
                            Question = "Does this change require inventory to be scrapped/written off?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Supply Chain",
                            Question = "Does this change impact raw material usage quantities (increase/decrease)?",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Training",
                            Question = "Is training required for this change? ",
                            Order = "05",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ImpactAssessmentResponseQuestions
                        {
                            ReviewType = "Training",
                            Question = "Will this training be required for break-in training binders? (new SOPs need to be added to the list in the binder)",
                            Order = "10",
                            CreatedUser = "MJWilson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Impact Assessment Response Questions.....
                if (!context.AllowedAttachmentExtensions.Any())
                {
                    context.AllowedAttachmentExtensions.AddRange(
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "doc",
                            Description = "Microsoft Word document before Word 2007"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "docx",
                            Description = "Microsoft Word document"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "dot",
                            Description = "Microsoft Word template before Word 2007"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "dotx",
                            Description = "Microsoft Word template"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "gif",
                            Description = "Graphical Interchange Format file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "jpeg",
                            Description = "Joint Photographic Experts Group photo file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "jpg",
                            Description = "Joint Photographic Experts Group photo file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "ods",
                            Description = "OpenDocument Spreadsheet"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "pdf",
                            Description = "Portable Document Format file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "png",
                            Description = "Portable Network Graphics file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "ppt",
                            Description = "Microsoft PowerPoint format before PowerPoint 2007"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "pptx",
                            Description = "Microsoft PowerPoint presentation"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "psd",
                            Description = "Adobe Photoshop file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "rtf",
                            Description = "Rich Text Format file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "tif",
                            Description = "Tagged Image Format file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "tiff",
                            Description = "Tagged Image Format file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "txt",
                            Description = "Unformatted text file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "vsd",
                            Description = "Microsoft Visio drawing before Visio 2013"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "vsdx",
                            Description = "Microsoft Visio drawing file"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "xls",
                            Description = "Microsoft Excel workbook before Excel 2007"
                        },
                        new AllowedAttachmentExtensions
                        {
                            ExtensionName = "xlsx",
                            Description = "Microsoft Excel workbook after Excel 2007"
                        }
                    );
                    context.SaveChanges();
                }



            }
        }
    }
}
