using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHS.Models
{
    public class seg_risk_assessment : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location is required.")]
        public string? location { get; set; }                    // auburn, bay city, all
        [Display(Name = "Exposure Type")]
        [Required(ErrorMessage = "Exposure Type is required.")]
        public string? exposure_type { get; set; }
        [Display(Name = "Agent")]
        [Required(ErrorMessage = "Agent is required.")]
        public string? agent { get; set; }
        [Display(Name = "Role/Job")]
        [Required(ErrorMessage = "Role/Job is required.")]
        public string? role { get; set; }
        [Display(Name = "Task")]
        [Required(ErrorMessage = "Task is required.")]
        public string? task { get; set; }
        [Display(Name = "Frequency of Task")]
        [Required(ErrorMessage = "Frequency of Task is required.")]
        public string? frequency_of_task { get; set; }
        [Display(Name = "Duration of Task")]
        [Required(ErrorMessage = "Duration of Task is required.")]
        public TimeSpan duration_of_task { get; set; }
        [Display(Name = "Occupational Exposure Limit")]
        public string? oel { get; set; }
        [Display(Name = "Acute/Chronic")]
        public string? acute_chronic { get; set; }
        [Display(Name = "Route of Entry(s)")]
        [Required(ErrorMessage = "Route of Entry is required.")]
        public List<string>? route_of_entry { get; set; }
        [Display(Name = "Monitoring Data Required")]
        [Required(ErrorMessage = "Monitoring Data is required.")]
        public string? monitoring_data_required { get; set; }
        [Display(Name = "Controls Recommended")]
        public List<string>? controls_recommended { get; set; }
        [Display(Name = "Exposure Levels Acceptable")]
        public string? exposure_levels_acceptable { get; set; }
        [Display(Name = "Date Conducted")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date Conducted is required.")]
        public DateTime? date_conducted { get; set; }
        [Display(Name = "Assessment Methods Used")]
        [Required(ErrorMessage = "Assessment Methods Used is required.")]
        public string? assessment_methods_used { get; set; }
        [Display(Name = "SEG Number of Workers")]
        [Required(ErrorMessage = "SEG Number of Workers is required.")]
        public string? seg_number_of_workers { get; set; }
        [Display(Name = "Has Agent been Changed")]
        public string? has_agent_been_changed { get; set; }
        [Display(Name = "Person Performing Assessment")]
        [Required(ErrorMessage = "Person Performing Assessment is required.")]
        public string? person_performing_assessment_username { get; set; }
        [Display(Name = "Person Performing Assessment")]
        public string? person_performing_assessment_displayname { get; set; }
        [Display(Name = "Exposure Rating")]
        [Range(0, 4)]
        public int? exposure_rating { get; set; }
        [Display(Name = "Exposure Rating")]
        public string? exposure_rating_description { get; set; }
        [Display(Name = "Health Effect Rating")]
        [Range(0, 4)]
        public int? health_effect_rating { get; set; }
        [Display(Name = "Health Effect Rating")]
        public string? health_effect_rating_description { get; set; }
        [Display(Name = "Risk Score")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? risk_score { get; set; }
        [Display(Name = "Additional Notes")]
        public string? additional_notes { get; set; }
    }
}
