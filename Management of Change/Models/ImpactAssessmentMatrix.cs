using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string ChangeType { get; set; }
        public string ReviewType { get; set; }
    }
}
