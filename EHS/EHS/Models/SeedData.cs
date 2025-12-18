using EHS.Data;
using Microsoft.EntityFrameworkCore;
using EHS.Models.Dropdowns;
using EHS.Models.Dropdowns.SEG;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;

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
                // Look for any Similar Exposure Group (SEG) Risk Assessments....
                if (!context.seg_risk_assessment.Any())
                {
                    //for (int i = 0; i < 22; i++)
                    //{
                    context.seg_risk_assessment.AddRange(
                        new seg_risk_assessment
                        {
                            location = "Bay City",
                            exposure_type = "Chemical",
                            agent = "Sulfuric Acid",                            
                            role = "Fabrication Operator",
                            task =  "Twisting",
                            oel = "Mercury: OEL ACGIH TLV TWA: 0.025 mg/m3 SKIN, OSHA PEL Ceiling 0.1 mg/m3, STEL: 0.03 mg/m3.  IDLH is 10 mg/m3",
                            //acute_chronic = "Acute",
                            route_of_entry = new List<string> { "Dermal-irritation" },
                            frequency_of_task = "Several times a day",
                            duration_of_task = new TimeSpan(0,0,0,0),
                            //monitoring_data_required = "Priority 2",
                            controls_recommended = new List<string> { "Engineering Controls - Ventilation" },
                            exposure_levels_acceptable = "Yes",
                            date_conducted = DateTime.Now.AddDays(-45),
                            date_reviewed = DateTime.Now.AddDays(-2000),
                            assessment_methods_used = "Employee interviews",
                            seg_number_of_workers = "1 to 4 workers",
                            has_agent_been_changed = "Yes - Quantity Used",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            exposure_rating = 1,
                            exposure_rating_description = "< 10% of the OEL",
                            health_effect_rating = 1,
                            health_effect_rating_description = "Reversable health effects of concern",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_risk_assessment
                        {
                            location = "Auburn",
                            exposure_type = "Ergonomic",
                            agent = "Ergonomic",
                            role = "Fabrication Operator",
                            task = "Awkward Posture",
                            oel = "Oxalic Acid: 1 mg/m3 ACGIH TLV & OSHA PEL",
                            //acute_chronic = "Acute",
                            route_of_entry = new List<string> { "Ingestion" },
                            frequency_of_task = "1-2x a month",
                            duration_of_task = new TimeSpan(1, 1, 1, 1),
                            //monitoring_data_required = "Priority 2",
                            controls_recommended = new List<string> { "Change people location 2" },
                            exposure_levels_acceptable = "No",
                            date_conducted = DateTime.Now,
                            date_reviewed = DateTime.Now.AddDays(-1810),
                            assessment_methods_used = "Personal Monitoring",
                            seg_number_of_workers = "21 to 50 workers",
                            has_agent_been_changed = "Yes - Concentration ",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            exposure_rating = 2,
                            exposure_rating_description = "Between 10% and 50% of the OEL",
                            health_effect_rating = 2,
                            health_effect_rating_description = "Severe, reversable health effects",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_risk_assessment
                        {
                            location = "Both",
                            exposure_type = "Noise",
                            agent = "Noise",
                            role = "Fabrication Operator",
                            task = "Area Noise",
                            oel = "Caustic Potash: KOH ACGIH TLV 2 mg/m3",
                            //acute_chronic = "Chronic",
                            route_of_entry = new List<string> { "Dermal-burn" },
                            frequency_of_task = "Quarterly or less frequent than that",
                            duration_of_task = new TimeSpan(2, 2, 2, 2),
                            //monitoring_data_required = "Priority 4",
                            controls_recommended = new List<string> { "Change people location" },
                            exposure_levels_acceptable = "No",
                            date_conducted = DateTime.Now.AddDays(-7),
                            date_reviewed = DateTime.Now.AddDays(-7),
                            assessment_methods_used = "FiDirect read measurementsller",
                            seg_number_of_workers = "51+ workers",
                            has_agent_been_changed = "No",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            exposure_rating = 3,
                            exposure_rating_description = "Between 50% and 100% of the OEL",
                            health_effect_rating = 4,
                            health_effect_rating_description = "Life Threatening or disabling injury or illness",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                    //};
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// SEED SIMILAR EXPOSURE GROUP (SEG) RISK ASSESSMENT DROPDOWNS
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Look for any Locations....
                if (!context.location.Any())
                {
                    context.location.AddRange(
                        new location
                        {
                            description = "Auburn",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new location
                        {
                            description = "Bay City",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new location
                        {
                            description = "Both",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Exposure Types....
                if (!context.exposure_type.Any())
                {
                    context.exposure_type.AddRange(
                        new exposure_type
                        {
                            description = "Chemical",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_type
                        {
                            description = "Ergonomic",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_type
                        {
                            description = "Noise",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_type
                        {
                            description = "Radiation",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any Agents....
                if (!context.agent.Any())
                {
                    context.agent.AddRange(
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Acetone",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Manganese: Potassium Permanganate",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Manganese: Sodium Permanganate",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Sulfuric Acid",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Potassium Hydroxide",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Hydrofluoric Acid Hydroxide",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Aluminum Oxide",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Graphite",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Caustic Soda",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Nitric Acid",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Ethanediol (Cool Concentrate)",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Propane - 1,2 Diol  (Cry-Tek 100 Antifreeze)",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Phtahalic Anhydride (Crystal Bond)",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Propylene Glycol (Dow Frost's)",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Ethylene Gas",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Chlorodifluoromethane (HCFC-22)",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Hydrochloric Acid",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Hydrogen Peroxide",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Isopropyl Alcohol",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Mercury",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Mineral Spirits",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Oxalic Acid",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "MIBK",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Caustic Potash: KOH",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        //new agent
                        //{
                        //    exposure_type = "Chemical",
                        //    description = "Ammonia",
                        //    sort_order = null,
                        //    display = true,
                        //    created_user = "MJWilson",
                        //    created_date = DateTime.Now
                        //},
                        new agent
                        {
                            exposure_type = "Ergonomic",
                            description = "Ergonomic",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Noise",
                            description = "Noise",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Radiation",
                            description = "Radiation",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any SEG Roles....
                if (!context.seg_role.Any())
                {
                    context.seg_role.AddRange(
                        new seg_role
                        {
                            description = "Growth Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Fabrication Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Fabrication Operator MWS",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Grind Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Seed Coat Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Polish Operator GSP",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Polish Operator GDR",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "C&M Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "KOH Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Sort/Pack Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "EPI Operator",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Growth Tech",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Tech Lathe",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Boule Anneal",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics Material Transporter",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Receiving Clerk",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Shipping Clerk",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Inventory Specialist",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Facilities Calibration Tech",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: E&I Tech/Electrician",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: HVAC Tech",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Tech Advisor 3-2-2",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Tech Advisor Days",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Millwright",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Engineer",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Administrative Staff",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any tasks....
                if (!context.task.Any())
                {
                    context.task.AddRange(
                        new task
                        {
                            description = "Lifting: less than 50 lbs",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Lifting: more than 50 lbs",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Twisting",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Awkward Posture",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Excessive Force",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Area Noise",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Personal Noise",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Direct Read Monitoring",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Line & Equipment Opening",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Coating",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Chemical Addition",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Making a Batch",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Filter Change",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Drum Change",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Jug Change",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Clean Out",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Connect/Disconnect",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Maintenance Other",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any occupational exposure limits....
                if (!context.occupational_exposure_limit.Any())
                {
                    context.occupational_exposure_limit.AddRange(
                        new occupational_exposure_limit
                        {
                            description = "Oil Mist TWA/PEL: 5 mg/m3, ACGIH STEL 10 mg/m3",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Manganese: ACGIH TLV 0.1 mg/m3 (inhalable fraction)",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Manganese: TWA 0.02 mg/m3 (respirable fraction) ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Potassium Hydroxide Ceiling: NIOSH REL 2 mg/m3 ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Sulfuric Acid: OSHA & NIOSH 1 mg/m3",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Hydrofluoric Acid: OSHA PEL 2.5 mg/m3, TWA 3 ppm, and NIOSH TWA 3 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Acetone: ACGIH TLV 500 ppm, STEL 750 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Aluminum Oxide: OSHA PEL TWA 15 mg/m3 (total), TWA 5 mg/m3 (resp).  ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Graphite: ACGIH 2 mg/m3 TWA Respirable Fraction, 5 mg/m3 PEL. ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Caustic Soda: 2 mg/m3 ACGIH and OSHA",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Nitric Acid: ACGIH OEL TWA 2 ppm, STEL 4 ppm ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Ethanediol ACGIH STEL 50 ppm (vapor) and 10 mg/m3 Inhalable Particulate), TLV TWA: 25 ppm vapor fraction",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Phthalic Anhydride NIOSH REL: TLV 1 ppm, OSHA PEL 2 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Ethylene Gas: ACGIH TWA 200 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Chlorodifluoromethane (HCFC-22): OEL TWA (ACGIH) 1000 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = " Hydrochloric Acid: ACGIH TLV-C 2 ppm, OSHA PEL-T/C 5 ppm. ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Hydrogen Peroxide ACGIH TWA 1 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Isopropyl Alcohol: NIOSH/OSHA: TWA of 400 ppm ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Mercury: OEL ACGIH TLV TWA: 0.025 mg/m3 SKIN, OSHA PEL Ceiling 0.1 mg/m3, STEL: 0.03 mg/m3. IDLH is 10 mg/m3",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Mineral Spirits: OSHA PEL TWA 500 ppm, NIOSH REL 350 ppm ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Oxalic Acid: 1 mg/m3 ACGIH TLV & OSHA PEL",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "MIBK: ACGIH TLV 20 ppm, STEL 75 ppm",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Caustic Potash: KOH ACGIH TLV 2 mg/m3",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any acute_chronic records....
                if (!context.acute_chronic.Any())
                {
                    context.acute_chronic.AddRange(
                        new acute_chronic
                        {
                            description = "Acute",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new acute_chronic
                        {
                            description = "Chronic",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // prefill route of entry
                if (!context.route_of_entry.Any())
                {
                    context.route_of_entry.AddRange(
                        new route_of_entry
                        {
                            description = "Dermal Absorbtion",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Dermal-Irritation",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Ingestion",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Inhalation-Particles",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Inhalation-Vapor or Mist",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Eye Absorption",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Dermal-Burn",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Hearing",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "X-Ray",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Injection",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any frequency of task records....
                if (!context.frequency_of_task.Any())
                {
                    context.frequency_of_task.AddRange(
                        new frequency_of_task
                        {
                            description = "Several times a day",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "Several times a week",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "1-2x a month",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "Quarterly or less frequent than that",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any monitoring data required records....
                if (!context.monitoring_data_required.Any())
                {
                    context.monitoring_data_required.AddRange(
                        new monitoring_data_required
                        {
                            description = "Priority 1",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new monitoring_data_required
                        {
                            description = "Priority 2",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new monitoring_data_required
                        {
                            description = "Priority 3",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new monitoring_data_required
                        {
                            description = "Priority 4",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any controls recommended records....
                if (!context.controls_recommended.Any())
                {
                    context.controls_recommended.AddRange(
                        new controls_recommended
                        {
                            description = "Engineering Controls - Ventilation",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "Engineering Controls - Noise Reduction Efforts",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "PPE - Hearing Protection in Specific Area",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "PPE - Change to Respiratory Protection",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "Change Equipment Location",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "Research Alternate Chemical.",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "Update of Process Documentation",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new controls_recommended
                        {
                            description = "Job Rotation",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any yes/no records....
                if (!context.yes_no.Any())
                {
                    context.yes_no.AddRange(
                        new yes_no
                        {
                            description = "Yes",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new yes_no
                        {
                            description = "No",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any assessment methods used records....
                if (!context.assessment_methods_used.Any())
                {
                    context.assessment_methods_used.AddRange(
                        new assessment_methods_used
                        {
                            description = "Employee Interviews",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new assessment_methods_used
                        {
                            description = "Personal Monitoring",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new assessment_methods_used
                        {
                            description = "Area Monitoring",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new assessment_methods_used
                        {
                            description = "Direct Read Measurements",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new assessment_methods_used
                        {
                            description = "Investigate Work Task Among SEG",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any number of workers records....
                if (!context.number_of_workers.Any())
                {
                    context.number_of_workers.AddRange(
                        new number_of_workers
                        {
                            description = "1 to 4 workers",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new number_of_workers
                        {
                            description = "5 to 10 workers",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new number_of_workers
                        {
                            description = "11 to 20 workers",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new number_of_workers
                        {
                            description = "21 to 50 workers",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new number_of_workers
                        {
                            description = "51+ workers",
                            sort_order = "50",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'Has Agent Been Changed' records....
                if (!context.has_agent_been_changed.Any())
                {
                    context.has_agent_been_changed.AddRange(
                        new has_agent_been_changed
                        {
                            description = "Yes - Quantity Used",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new has_agent_been_changed
                        {
                            description = "Yes - How Often It Is Used",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new has_agent_been_changed
                        {
                            description = "Yes - Concentration ",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new has_agent_been_changed
                        {
                            description = "No",
                            sort_order = null,
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'Exposure Rating' records....
                if (!context.exposure_rating.Any())
                {
                    context.exposure_rating.AddRange(
                        new exposure_rating
                        {
                            value = 1,
                            description = "< 10% of the OEL",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_rating
                        {
                            value = 2,
                            description = "Between 10% and 50% of the OEL",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_rating
                        {
                            value = 3,
                            description = "Between 50% and 100% of the OEL",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new exposure_rating
                        {
                            value = 4,
                            description = ">100% of the OEL",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'Health Effect Rating' records....
                if (!context.health_effect_rating.Any())
                {
                    context.health_effect_rating.AddRange(
                        new health_effect_rating
                        {
                            value = 1,
                            description = "Reversable Health Effects of Concern",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new health_effect_rating
                        {
                            value = 2,
                            description = "Severe, Reversable Health Effects",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new health_effect_rating
                        {
                            value = 3,
                            description = "Irreversible Health Effects of Concern",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new health_effect_rating
                        {
                            value = 4,
                            description = "Life Threatening or Disabling Injury or Illness",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }


                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// SEED CHEMICAL RISK ASSESSMENT DROPDOWNS
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Look for any 'area' records....
                if (!context.area.Any())
                {
                    context.area.AddRange(
                        new area
                        {
                            description = "C&M",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "EPI",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Fabrication",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Fabrication MWS",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Fabrication MWS, Maintenance",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Fabrication, Maintenance",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Facilities",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Facilities, Maintenance",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "GDR Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "GDR Polish, C&M",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Grind",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth Operations, R&D",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth, R&D",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth, Fabrication, Seed Coat",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth, Facilities",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Growth, Facilities, EPI",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "GSP, Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "KOH",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Polish GDR",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Polish GDR, GSP",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Polish GSP",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Quality",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "R&D Lab",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "R&D Lab & R&D Growth",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new area
                        {
                            description = "Seed Coat",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'use' records....
                if (!context.use.Any())
                {
                    context.use.AddRange(
                        new use
                        {
                            description = "2-5 Grams in Crucible",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Biocide",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Compressor Fluid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "EPI",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Fab/Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Fab: Studer Oil",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "GDR Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Facilities, Maintenance",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "GDR Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "GDR Polish Slurry",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "KOH Bath (seed coat)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "KOH Bench",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Lab Chemical",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Maintenance and ?? (base oil)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Maintenance and ?? (circulating/gear oil)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Maintenance and ?? (grease)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Maintenance and ?? (lubricant)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Maintenance and ?? (turbine oil)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Polish",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Polish: Terminator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Process Aid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Process Aid (DAC)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Process Aid to remove Metals",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Process Aid: Terminator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Raw Material",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Slurry",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Tepla Furnace compressor lubricant",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Tepla Furnace vacuum pump oil",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Waste",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new use
                        {
                            description = "Water Treatment",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'physical state' records....
                if (!context.physical_state.Any())
                {
                    context.physical_state.AddRange(
                        new physical_state
                        {
                            description = "Solid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new physical_state
                        {
                            description = "Liquid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new physical_state
                        {
                            description = "Gas",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'physical state' records....
                if (!context.hazardous.Any())
                {
                    context.hazardous.AddRange(
                        new hazardous
                        {
                            description = "Hazardous",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazardous
                        {
                            description = "Non-Hazardous",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'ppe-eye' records....
                if (!context.ppe_eye.Any())
                {
                    context.ppe_eye.AddRange(
                        new ppe_eye
                        {
                            description = "None Required",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_eye
                        {
                            description = "Safety Glasses (with side shields)",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_eye
                        {
                            description = "Safety Goggles (splash protection)",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_eye
                        {
                            description = "Face Shield (to be worn over safety glasses/goggles)",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_eye
                        {
                            description = "Welding Goggles/Shield (for specific light hazards)",
                            sort_order = "50",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_eye
                        {
                            description = "Other (specify in notes) ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'ppe-glove' records....
                if (!context.ppe_glove.Any())
                {
                    context.ppe_glove.AddRange(
                        new ppe_glove
                        {
                            description = "None Required",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Disposable Nitrile Gloves (e.g., incidental contact)",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Disposable Latex Gloves (use with caution due to allergy potential)",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Chemical-Resistant Gloves (e.g., butyl rubber, neoprene, PVC; requires specific compatibility check via SDS)",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Inner/Outer Chemical-Resistant Gloves (for enhanced protection/Level A, B, C)",
                            sort_order = "50",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Cut-Resistant Gloves (for physical hazards)",
                            sort_order = "60",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Heat-Resistant Gloves",
                            sort_order = "70",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_glove
                        {
                            description = "Other (specify material and thickness in notes) ",
                            sort_order = "80",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'ppe-Suit' records....
                if (!context.ppe_suit.Any())
                {
                    context.ppe_suit.AddRange(
                        new ppe_suit
                        {
                            description = "None Required",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Lab Coat (flame-resistant if handling flammable chemicals)",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Chemical-Resistant Apron",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Coveralls (e.g., Tyvek, disposable, or flame-resistant)",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Hooded Chemical-Resistant Suit (e.g., PVC splash suit for Level C)",
                            sort_order = "50",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Totally-Encapsulating Chemical-Protective Suit (Vapor protective suit for Level A)",
                            sort_order = "60",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_suit
                        {
                            description = "Other (specify in notes) ",
                            sort_order = "70",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'ppe-Respiratory' records....
                if (!context.ppe_respiratory.Any())
                {
                    context.ppe_respiratory.AddRange(
                        new ppe_respiratory
                        {
                            description = "None Required",
                            sort_order = "10",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Dust Mask (basic nuisance dust, not for chemicals)",
                            sort_order = "20",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Half-Mask Air-Purifying Respirator (APR) (requires specific cartridge/filter for known contaminant and fit-testing)",
                            sort_order = "30",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Full-Face Air-Purifying Respirator (APR) (requires specific cartridge/filter for known contaminant and fit-testing)",
                            sort_order = "40",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Supplied-Air Respirator (SAR) / Air-Line Respirator (for oxygen-deficient or highly toxic atmospheres)",
                            sort_order = "50",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Self-Contained Breathing Apparatus (SCBA) (positive pressure, full-facepiece, for unknown/immediately dangerous to life and health (IDLH) atmospheres)",
                            sort_order = "60",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Escape SCBA (used with SAR)",
                            sort_order = "70",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new ppe_respiratory
                        {
                            description = "Other (specify type and filter/cartridge in notes)",
                            sort_order = "80",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }

                // Look for any 'hazard codes' records....
                if (!context.hazard_codes.Any())
                {
                    context.hazard_codes.AddRange(
                        new hazard_codes
                        {
                            code = "H281",
                            description = "Contains refrigerated gas; may cause cryogenic burns or injury",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H300",
                            description = "Fatal if swallowed",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H301",
                            description = "Toxic if swallowed",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H302",
                            description = "Harmful if swallowed",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H303",
                            description = "May be harmful if swallowed",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H304",
                            description = "May be fatal if swallowed and enters airway",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H305",
                            description = "May be harmful if swallowed and enters airway",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H311",
                            description = "Toxic in contact with Skin",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H312",
                            description = "Harmful in contact with skin",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H313",
                            description = "May be harmful in contact with skin",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H314",
                            description = "Causes severe skin burns and eye damage",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H315",
                            description = "Causes skin irritation",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H316",
                            description = "Causes mild skin irritation",
                            sort_order = "",
                            display = true,
                            risk_rating = 1,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H317",
                            description = "May cause an allergic skin reaction",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H318",
                            description = "Causes serious eye damage",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H319",
                            description = "Causes serious eye irritation",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H320",
                            description = "Causes eye irritation",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H330",
                            description = "Fatal if inhaled",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H331",
                            description = "Toxic if inhaled",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H332",
                            description = "Harmful if inhaled",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H333",
                            description = "May be harmful inhaled",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H334",
                            description = "May cause allergy or asthma symptons or breathing difficulties if inhaled",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H335",
                            description = "May cause respiratory irritation",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H336",
                            description = "May cause drowsiness or dizziness",
                            sort_order = "",
                            display = true,
                            risk_rating = 2,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H340",
                            description = "May cause genetic defects",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H341",
                            description = "Suspected of causing genetic defects",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H350",
                            description = "May cause cancer",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H351",
                            description = "Suspected of causing cancer **",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H360",
                            description = "May damage fertility or the unborn child",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H361",
                            description = "Suspected of damaging fertility or the unborn child",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H362",
                            description = "May cause harm to breast-fed children",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H370",
                            description = "Causes damage to organs H372 Causes damage to organs through prolonged or repeated exposure",
                            sort_order = "",
                            display = true,
                            risk_rating = 4,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H371",
                            description = "May cause damage to organs",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new hazard_codes
                        {
                            code = "H373",
                            description = "May cause damage to organs through prolonged or repeated exposure",
                            sort_order = "",
                            display = true,
                            risk_rating = 3,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }


            }
        }
    }
}
