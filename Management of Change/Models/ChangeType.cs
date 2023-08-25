using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ChangeType : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? Order { get; set; }
    }
}
