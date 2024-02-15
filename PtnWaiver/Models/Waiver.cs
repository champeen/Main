using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PtnWaiver.Models
{
    public class Waiver : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Waiver Number")]
        public string? WaiverNumber { get; set; }
        [Display(Name = "Revision Number")]
        public int RevisionNumber { get; set; }
        [Display(Name = "POR or Project")]
        public string PorProject { get; set; }
        public string Description { get; set; }
        [Display(Name = "Product/Process")]
        public string ProductProcess { get; set; }
        public string Status { get; set; }
        [Display(Name = "Date Closed")]
        [DataType(DataType.Date)]
        public DateTime? DateClosed { get; set; }
        [Display(Name = "Corrective Action Due Date")]
        [DataType(DataType.Date)]
        public DateTime? CorrectiveActionDueDate { get; set; }

        [ForeignKey("PTN")]
        public int PTNId { get; set; }
        public string? PtnDocId { get; set; }
        public virtual PTN? PTN {  get; set; }
    }
}
