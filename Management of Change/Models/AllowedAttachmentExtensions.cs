using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class AllowedAttachmentExtensions
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Extension Name")]
        public string ExtensionName { get; set; }
    }
}
