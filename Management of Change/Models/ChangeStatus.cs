using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ChangeStatus : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public bool Default { get; set; }
        public string? Order { get; set; }
    }
}
