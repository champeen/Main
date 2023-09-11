using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class PTN : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Enabled { get; set; }
        public string? Order { get; set; }
    }
}
