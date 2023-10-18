using Management_of_Change.Models;
using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.ViewModels
{
    public class CancelChangeRequest
    {
        [Display(Name = "MoC Number")]
        public string MocNumber { get; set; }
        public string CancelReason { get; set; }
        public ChangeRequest? ChangeRequest { get; set; }
    }
}
