using EHS.Data;
using Microsoft.EntityFrameworkCore;
using EHS.Models.Dropdowns;

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
                            seg_role = "Fabrication Operator",
                            task = "Twisting",
                            oel = "Mercury: OEL ACGIH TLV TWA: 0.025 mg/m3 SKIN, OSHA PEL Ceiling 0.1 mg/m3, STEL: 0.03 mg/m3.  IDLH is 10 mg/m3",
                            acute_chronic = "Acute",
                            route_of_entry = "Dermal-irritation",
                            frequency_of_task = "Several times a day",
                            duration_of_task = "STEL: 15 - 30 min",
                            monitoring_data_required = "Priority 2",
                            controls_recommended = "Engineering Controls- Ventilation",
                            exposure_levels_acceptable = "Yes",
                            date_conducted = DateTime.Now.AddDays(-45),
                            assessment_methods_used = "Employee interviews",
                            seg_number_of_workers = 5,
                            has_agent_been_changed = "yes-quantity used",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_risk_assessment
                        {
                            location = "Auburn",
                            exposure_type = "Ergonomic",
                            agent = "Ergonomic",
                            seg_role = "Fabrication Operator",
                            task = "Awkward Posture",
                            oel = "Oxalic Acid: 1 mg/m3 ACGIH TLV & OSHA PEL",
                            acute_chronic = "Acute",
                            route_of_entry = "Ingestion",
                            frequency_of_task = "1-2x a month",
                            duration_of_task = "STEL: 15 - 30 min",
                            monitoring_data_required = "Priority 2",
                            controls_recommended = "Change people location 2",
                            exposure_levels_acceptable = "No",
                            date_conducted = DateTime.Now,
                            assessment_methods_used = "Personal Monitoring",
                            seg_number_of_workers = 1,
                            has_agent_been_changed = "yes- concentration ",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_risk_assessment
                        {
                            location = "Both",
                            exposure_type = "Noise",
                            agent = "Noise",
                            seg_role = "Fabrication Operator",
                            task = "Area Noise",
                            oel = "Caustic Potash: KOH ACGIH TLV 2 mg/m3",
                            acute_chronic = "Chronic",
                            route_of_entry = "Dermal-burn",
                            frequency_of_task = "Quarterly or less frequent than that",
                            duration_of_task = "Less than 15 min",
                            monitoring_data_required = "Priority 4",
                            controls_recommended = "Change people location",
                            exposure_levels_acceptable = "No",
                            date_conducted = DateTime.Now.AddDays(-7),
                            assessment_methods_used = "FiDirect read measurementsller",
                            seg_number_of_workers = 7,
                            has_agent_been_changed = "No",
                            person_performing_assessment_username = "MJWilson",
                            person_performing_assessment_displayname = "Michael Wilson",
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                    //};
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// DROPDOWNS
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Acetone",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Manganese: Potassium permanganate",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Manganese: Sodium permanganate",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Sulfuric Acid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Potassium Hydroxide",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Hydrofluoric Acid Hydroxide",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Aluminum Oxide",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Graphite",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Caustic Soda",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Nitric Acid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Ethanediol (Cool Concentrate)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Propane - 1,2 diol  (Cry-Tek 100 Antifreeze)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Phtahalic Anhydride (Crystal bond)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Propylene glycol (Dow Frost's)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Ethylene gas",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Chlorodifluoromethane (HCFC-22)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Hydrochloric acid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Hydrogen Peroxide",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Isopropyl Alcohol",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Mercury",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Mineral Spirits",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Oxalic Acid",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "MIBK",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Caustic Potash: KOH",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Chemical",
                            description = "Ammonia",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Ergonomic",
                            description = "Ergonomic",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Noise",
                            description = "Noise",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new agent
                        {
                            exposure_type = "Radiation",
                            description = "Radiation",
                            sort_order = "",
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
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Fabrication Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Fabrication Operator MWS",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Grind Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Seed Coat Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Polish Operator GSP",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Polish Operator GDR",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "C&M Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "KOH Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Sort/Pack Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "EPI Operator",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Growth Tech",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Tech Lathe",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "RnD Boule Anneal",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics Material Transporter",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Receiving Clerk",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Shipping Clerk",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Logistics: Inventory Specialist",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Facilities Calibration Tech",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: E& I Tech/Electrician",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: HVAC Tech",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Tech Advisor 3-2-2",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Tech Advisor Days",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Maintenance: Millwright",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Engineer",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new seg_role
                        {
                            description = "Administrative Staff",
                            sort_order = "",
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
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Lifting: more than 50 lbs",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Twisting",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Awkward posture",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Excessive Force",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Area Noise",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Personal Noise",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Direct Read Monitoring",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Line & Equipment Opening",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Coating",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Chemical Addition",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Making a batch",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Filter Change",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Drum Change",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Jug Change",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Clean Out",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Connect/Disconnect",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new task
                        {
                            description = "Maintenance Other",
                            sort_order = "",
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
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Manganese: ACGIH TLV 0.1 mg/m3 (inhalable fraction)",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Manganese: TWA 0.02 mg/m3 (respirable fraction) ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Potassium Hydroxide Ceiling:   NIOSH REL 2 mg/m3 ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Sulfuric Acid: OSHA & NIOSH 1 mg/m3",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Hydrofluoric Acid: OSHA PEL 2.5 mg/m3, TWA 3 ppm, and NIOSH TWA 3 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Acetone: ACGIH TLV 500 ppm, STEL 750 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Aluminum Oxide: OSHA PEL TWA 15 mg/m3 (total), TWA 5 mg/m3 (resp).  ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Graphite: ACGIH 2 mg/m3 TWA respirable fraction, 5 mg/m3 PEL. ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Caustic Soda: 2 mg/m3 ACGIH and OSHA",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Nitric acid: ACGIH OEL TWA 2 ppm, STEL 4 ppm ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Ethanediol ACGIH STEL 50 ppm (vapor) and 10 mg/m3 inhalable particulate), TLV TWA: 25 ppm vapor fraction",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Phthalic Anhydride NIOSH REL: TLV 1 ppm, OSHA PEL 2 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Ethylene gas: ACGIH TWA 200 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Chlorodifluoromethane (HCFC-22): OEL TWA (ACGIH) 1000 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = " Hydrochloric acid: ACGIH TLV-C 2 ppm, OSHA PEL-T/C 5 ppm. ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Hydrogen peroxide ACGIH TWA 1 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Isopropyl Alcohol: NIOSH/OSHA: TWA of 400 ppm ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Mercury: OEL ACGIH TLV TWA: 0.025 mg/m3 SKIN, OSHA PEL Ceiling 0.1 mg/m3, STEL: 0.03 mg/m3.  IDLH is 10 mg/m3",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Mineral Spirits: OSHA PEL TWA 500 ppm, NIOSH REL 350 ppm ",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Oxalic Acid: 1 mg/m3 ACGIH TLV & OSHA PEL",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "MIBK: ACGIH TLV 20 ppm, STEL 75 ppm",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new occupational_exposure_limit
                        {
                            description = "Caustic Potash: KOH ACGIH TLV 2 mg/m3",
                            sort_order = "",
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
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new acute_chronic
                        {
                            description = "Chronic",
                            sort_order = "",
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
                            description = "Dermal absorbtion",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Dermal-irritation",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Ingestion",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Inhalation-particles",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Inhalation-vapor or mist",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Eye absorption",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Dermal-burn",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Hearing",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "X-Ray",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new route_of_entry
                        {
                            description = "Injection",
                            sort_order = "",
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
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "Several times a week",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "1-2x a month",
                            sort_order = "",
                            display = true,
                            created_user = "MJWilson",
                            created_date = DateTime.Now
                        },
                        new frequency_of_task
                        {
                            description = "Quarterly or less frequent than that",
                            sort_order = "",
                            display = true,
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
