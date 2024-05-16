using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ChangeLevel : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Grade")]
        public string Level { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Change Grade Review Required")]
        public bool ChangeGradeReviewRequired { get; set; }
        [Display(Name = "PCCB Review Required")]
        public bool PccbReviewRequired { get; set; }
        public string? Order { get; set; }
    }
}
