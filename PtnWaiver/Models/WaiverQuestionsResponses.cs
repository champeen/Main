using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PtnWaiver.Models
{
    public class WaiverQuestionResponse : TimeStamps
    {
        [Key]
        public int Id { get; set; }        
        public string Question { get; set; }
        [Display(Name = "Group/Approver(s)")]
        public List<string> GroupApprover { get; set; }
        public string? Order { get; set; }
        public string? Response { get; set; }

        [ForeignKey("Waiver")]
        public int WaiverId { get; set; }

        public virtual Waiver? Waiver { get; set; }
    }
}
