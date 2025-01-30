using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class EmailLists : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "List Name")]
        public string ListName { get; set; }
        [Display(Name = "Emails")]
        public List<string> Emails { get; set; }
        public string? Order { get; set; }
    }
}
