using System.ComponentModel.DataAnnotations;

namespace EHS.Models
{
    public class seg_risk_assessment : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Location")]
        public string location { get; set; }                    // auburn, bay city, all
        [Display(Name = "Exposure Type")]
        public string exposure_type { get; set; }
        [Display(Name = "Agent")]
        public string agent {  get; set; }
        [Display(Name = "SEG/Role")]
        public string seg_role {  get; set; }
        [Display(Name = "Task")]
        public string task { get; set; }
        [Display(Name = "OEL")]
        public string oel { get; set; }
        [Display(Name = "Acute/Chronic")]
        public string acute_chronic { get; set; }
        [Display(Name = "Route of Entry")]
        public string route_of_entry { get; set; }
        [Display(Name = "Frequency of Task")]
        public string frequency_of_task {  get; set; }
        [Display(Name = "Duration of Task")]
        public string duration_of_task { get; set; }
        [Display(Name = "Monitoring Data Required")]
        public string monitoring_data_required { get; set; }
        [Display(Name = "Controls Recommended")]
        public string controls_recommended { get; set; }
        [Display(Name = "Exposure Levels Acceptable")]
        public string exposure_levels_acceptable { get; set; }
        [Display(Name = "Date Conducted")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime? date_conducted { get; set; }
        [Display(Name = "Assessment Methods Used")]
        public string assessment_methods_used { get; set; }
        [Display(Name = "SEG Number of Workers")]
        public int seg_number_of_workers { get; set; }
        [Display(Name = "Has Agent been Changed")]
        public string has_agent_been_changed { get; set; }
        [Display(Name = "Person Performing Assessment")]
        public string person_performing_assessment_username { get; set; }
        public string? person_performing_assessment_displayname { get; set; }
    }
}
