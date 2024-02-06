using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class PtnPin : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? Order { get; set; }
    }
}
