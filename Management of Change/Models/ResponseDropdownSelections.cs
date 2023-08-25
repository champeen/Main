using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ResponseDropdownSelections : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Response { get; set; }
        public string? Order { get; set; }
    }
}
