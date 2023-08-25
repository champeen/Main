using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentResponseQuestions : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
        public string Question { get; set; }
        public string? Order { get; set; }

        //[ForeignKey("ChangeRequest")]
        //public int ImpactAssessmentResponseId { get; set; }
    }
}
