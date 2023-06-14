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
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(3),
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
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
                            Expiration_Date_Temporary = null,
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Fred Jones",
                            ModifiedDate = DateTime.Now.AddMonths(-3),
                            DeletedUser = "Jesse Girl",
                            DeletedDate = DateTime.Now.AddDays(-2)
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
                            Expiration_Date_Temporary = System.DateTime.Now.AddMonths(7),
                            CreatedUser = "Michael James Wilson II",
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
                            Order = "10",
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
                            Order = "15",
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
                            Order = "20",
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
                            Order = "25",
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
                            Order = "30",
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
                            Order = "35",
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            Order = "50",
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
                            Order = "55",
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
                            Order = "60",
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
                            Order = "65",
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
                            Order = "70",
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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

                // Look for any Change Status.....
                if (!context.ChangeStatus.Any())
                {
                    context.ChangeStatus.AddRange(
                        new ChangeStatus
                        {
                            Status = "Change Proposal",
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Change Evaluation",
                            Order = "2",
                            CreatedUser = "Joe Jackson",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Implementation",
                            Order = "3",
                            CreatedUser = "Steve Smith",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Verification",
                            Order = "4",
                            CreatedUser = "Fred Bear",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "On Hold",
                            Order = "5",
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
                            Order = "6",
                            CreatedUser = "Shelly Shelby",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeStatus
                        {
                            Status = "Killed",
                            Order = "7",
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
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ResponseDropdownSelections
                        {
                            Response = "No",
                            Order = "2",
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
                            Order = "3",
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
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "100mm bare",
                            Order = "2",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "150mm bare",
                            Order = "3",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "200mm bare",
                            Order = "4",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "All Epi",
                            Order = "5",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "100mm Epi",
                            Order = "6",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "150mm Epi",
                            Order = "7",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "200mm Epi",
                            Order = "8",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "165mm seed",
                            Order = "9",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ProductLine
                        {
                            Description = "Multiple",
                            Order = "91",
                            CreatedUser = "Michael James Wilson II",
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
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new SiteLocation
                        {
                            Description = "Auburn",
                            Order = "2",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new SiteLocation
                        {
                            Description = "Bay City",
                            Order = "3",
                            CreatedUser = "Michael James Wilson II",
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
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Fab",
                            Order = "2",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Grind",
                            Order = "3",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = "Joe Jackson",
                            ModifiedDate = DateTime.Now,
                            DeletedUser = "Jackson Browne",
                            DeletedDate = DateTime.Now
                        },
                        new ChangeArea
                        {
                            Description = "Polish",
                            Order = "4",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Clean & Metrology",
                            Order = "5",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Epi",
                            Order = "6",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Sort & Package",
                            Order = "7",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Disposition",
                            Order = "8",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new ChangeArea
                        {
                            Description = "Other",
                            Order = "9",
                            CreatedUser = "Michael James Wilson II",
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
                            Order = "1",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require the update of existing approved SOPs, Runsheets, Checklists, etc?  If yes, ensure they are included in the MOC folder and upon MOC approval are submitted through the proper Document Management System process for review. ",
                            Order = "2",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require temporary SOPs, runsheets or checklists?  If yes, ensure they are included in the MOC folder and upon MOC approval are submitted through the proper Document Management System process for review. ",
                            Order = "3",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require training?  If yes, ensure that the scope and justification of the training is provided in the Training Checklist and Impact Assessment is checked as required below.",
                            Order = "4",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change involve new chemicals or an increase in chemical quantities for the Site?  If yes, ensure that Environment and Health and Industrial Hygiene Impact Assessments are checked as required below.",
                            Order = "5",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require IT support for new hardware, software, increased data storage, or a update/add to MES?  If yes, ensure IT checklist is checked as required below.",
                            Order = "6",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require a new raw, intermediate, or Finished Goods material number?  If yes, ensure that the responsible Engineer has a task to begin the new material creation process in the Supply Chain Impact Assessment Checklist.\t\t\t\t\t\t\r\n",
                            Order = "7",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change require additional WIP packaging components?  If yes, provide information on how many in the Supply Chain Impact Assessment Checklist.",
                            Order = "8",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does the change impact existing raw material usage quantities (increase or decrease)?  If yes, provide detailed quantities in the Supply Chain Impact Assessment Checklist.",
                            Order = "9",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Is this change associated with a PTN?  If yes, please enter the PTN number(s) to the right.",
                            Order = "91",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Does this change require the use of a waiver for the release of final product?  If so, please enter the waiver number(s) to the right.",
                            Order = "92",
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        },
                        new GeneralMocQuestions
                        {
                            Question = "Is this change associated with a CMT (NOTE: all Level 1-3 changes require CMT)? If so, enter CMT number to the right and/or initiate CMT with CMT Leader.",
                            Order = "93",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
                            DeletedUser = null,
                            DeletedDate = null
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any ReviewTypes.....
                if (!context.ImpactAssessmentMatrix.Any())
                {
                    context.ImpactAssessmentMatrix.AddRange(
                        new ImpactAssessmentMatrix
                        {
                            ReviewType = "Device Expert",
                            ChangeType = "Equipment Installation",
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                           CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                           CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                           CreatedUser = "Michael James Wilson II",
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
                           CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
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
                            CreatedUser = "Michael James Wilson II",
                            CreatedDate = DateTime.Now,
                            ModifiedUser = null,
                            ModifiedDate = null,
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
