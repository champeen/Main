using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class GeneralMocQuestions : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        public string? Order { get; set; }
    }
}
