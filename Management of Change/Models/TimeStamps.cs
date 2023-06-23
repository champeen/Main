using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class TimeStamps
    {
        [Display(Name = "Create User")]
        public string CreatedUser { get; set; }
        [Display(Name = "Create Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified User")]
        public string? ModifiedUser { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Deleted User")]
        public string? DeletedUser { get; set; }
        [Display(Name = "Deleted Date")]
        public DateTime? DeletedDate { get; set; }
    }
}
