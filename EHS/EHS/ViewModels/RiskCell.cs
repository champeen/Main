using System.ComponentModel.DataAnnotations;

namespace EHS.ViewModels
{
    public class RiskCell
    {
        public int Severity { get; set; }
        public int Likelihood { get; set; }
        public int? Quantity { get; set; } // optional, if you want numbers inside
        public string Color { get; set; }   // "bg-danger", "bg-warning", "bg-success"
    }
}