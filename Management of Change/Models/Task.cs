using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class Task : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Change Request")]
        public int? ChangeRequestId { get; set; }
        [Display(Name = "MOC Number")]
        public string? MocNumber { get; set; }
        [Display(Name = "Implementation Type")]
        public string? ImplementationType { get; set; }
        public string Status { get; set; }
        public string? Priority { get; set; }
        [Display(Name = "Assigned To User")]
        public string AssignedToUser { get; set; }
        [Display(Name = "Assigned To User Full Name")]
        public string? AssignedToUserFullName { get; set; }
        [Display(Name = "Assigned To User Email")]
        public string? AssignedToUserEmail { get; set; }
        [Display(Name = "Assigned By User")]
        public string AssignedByUser { get; set; }
        [Display(Name = "Assigned By Full Name")]
        public string? AssignedByUserFullName { get; set; }
        [Display(Name = "Assigned By User Email")]
        public string? AssignedByUserEmail { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletionDate {get; set;}
        [Display(Name = "Completion Notes")]
        public string? CompletionNotes { get; set; }
        [Display(Name="On Hold Reason")]
        public string? OnHoldReason { get; set; }
        [Display(Name ="Cancelled Reason")]
        public string? CancelledReason { get; set; }
        [ForeignKey("ImpactAssessmentResponseAnswer")]
        public int? ImpactAssessmentResponseAnswerId { get; set; }
    }
}
