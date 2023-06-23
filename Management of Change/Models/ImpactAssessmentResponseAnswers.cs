using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentResponseAnswer : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
        public string Question { get; set; }
        public string? Order { get; set; }
        public string? Action { get; set; }
        [Display(Name = "Details of Action Needed")]
        public string? DetailsOfActionNeeded { get; set; }
        [Display(Name = "Pre or Post Implementation")]
        public string? PreOrPostImplementation { get; set; }
        [Display(Name = "Action Owner")]
        public string? ActionOwner { get; set; }
        [Display(Name = "Date Due")]
        public DateTime? DateDue { get; set; }
        [ForeignKey("ImpactAssessmentResponse")]
        public int ImpactAssessmentResponseId { get; set; }
    }
}
