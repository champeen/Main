using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class TimeStamps
    {
        [Display(Name = "Create Username")]
        public string CreatedUser { get; set; }
        [Display(Name = "Create User Full Name")]
        public string CreatedUserFullName { get; set; }
        [Display(Name = "Create User Email")]
        public string CreatedUserEmail { get; set; }
        [Display(Name = "Create Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified Username")]
        public string? ModifiedUser { get; set; }
        [Display(Name = "Modified Usern Full Name")]
        public string? ModifiedUserFullName { get; set; }
        [Display(Name = "Modified User Email")]
        public string? ModifiedUserEmail { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Deleted Username")]
        public string? DeletedUser { get; set; }
        [Display(Name = "Deleted User Full Name")]
        public string? DeletedUserFullName { get; set; }
        [Display(Name = "Deleted User Email")]
        public string? DeletedUserEmail { get; set; }
        [Display(Name = "Deleted Date")]
        public DateTime? DeletedDate { get; set; }
    }
}
