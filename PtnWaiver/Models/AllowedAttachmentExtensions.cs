using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class AllowedAttachmentExtensions
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Extension Name")]
        public string ExtensionName { get; set; }
        public string? Description { get; set; }
    }
}
