using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class EmailHistory : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string? Priority { get; set; }
        [DisplayName("Sent To Name")]
        public string? SentToDisplayName { get; set; }
        [DisplayName("Sent To Username")]
        public string? SentToUsername { get; set; }
        [DisplayName("Sent To Email")]
        public string? SentToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public int? PtnId { get; set; }
        public int? WaiverId { get; set; }
        public int? TaskId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
    }
}
