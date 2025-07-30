using System.ComponentModel.DataAnnotations;

namespace EHS.Models.Dropdowns
{
    public class route_of_entry : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Sort Order")]
        public string? sort_order { get; set; }
        [Display(Name = "Display in Select List")]
        public bool display {  get; set; }
    }
}
