using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class AdditionalImpactAssessmentReviewersVM
    {
        public List<AdditionalImpactAssessmentReviewers> AdditionalImpactAssessmentReviewers { get; set; }
        public int ChangeRequestId { get; set; }
        public string Tab {  get; set; }   
        public string? EquipmentReviewerRequired { get; set; }
        public string? MaintenanceReviewerRequired { get; set; }
        public List<AdditionalImpactAssessmentReviewers>? EquipmentReviewers { get; set; }
        public List<AdditionalImpactAssessmentReviewers>? MaintenanceReviewers { get; set; }
        public ChangeRequest? ChangeRequest { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
