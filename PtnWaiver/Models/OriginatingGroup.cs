using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class OriginatingGroup : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Product Size Required")]
        public bool ProductSizeRequired { get; set; }
        public string? Order { get; set; }
    }
}
