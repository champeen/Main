using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class PccbStep : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string? Order { get; set; }
    }
}
