using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class PtnStatus : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public bool Default { get; set; }
        public string? Order { get; set; }
    }
}
