using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class WaiverQuestion : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        [Display(Name = "Group/Approver(s)")]
        public List<string> GroupApprover { get; set; }
        public string? Order { get; set; }
    }
}
