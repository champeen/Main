using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
    }
}
