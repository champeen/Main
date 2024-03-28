using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class OriginatingGroup : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public bool BouleSizeRequired { get; set; }
        public string? Order { get; set; }
    }
}
