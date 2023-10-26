using Management_of_Change.Models;
namespace Management_of_Change.ViewModels
{
    public class ImpactAssessmentResponseViewModel
    {
        public ChangeRequest? ChangeRequest { get; set; }
        public ImpactAssessmentResponse? ImpactAssessmentResponse { get; set; }
        public string? IARrecord { get; set; }
    }
}
