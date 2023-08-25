using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ReviewType : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Reviewer { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Order { get; set; }
    }
}
