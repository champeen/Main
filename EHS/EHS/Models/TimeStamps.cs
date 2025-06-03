using System.ComponentModel.DataAnnotations;

namespace EHS.Models
{
    public class TimeStamps
    {
        [Display(Name = "Create Username")]
        public string created_user { get; set; }
        [Display(Name = "Create User")]
        public string? created_user_fullname { get; set; }
        [Display(Name = "Create User Email")]
        public string? created_user_email { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime created_date { get; set; }
        [Display(Name = "Modified Username")]
        public string? modified_user { get; set; }
        [Display(Name = "Modified User")]
        public string? modified_user_fullname { get; set; }
        [Display(Name = "Modified User Email")]
        public string? modified_user_email { get; set; }
        [Display(Name = "Modified Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? modified_date { get; set; }
        [Display(Name = "Deleted Username")]
        public string? deleted_user { get; set; }
        [Display(Name = "Deleted User")]
        public string? deleted_user_fullname { get; set; }
        [Display(Name = "Deleted User Email")]
        public string? deleted_user_email { get; set; }
        [Display(Name = "Deleted Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? deleted_date { get; set; }
    }
}
