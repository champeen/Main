using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PtnWaiver.Models
{
    public class Waiver : TimeStamps
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [ForeignKey("PTN")]
        public int PTNId { get; set; }
    }
}
