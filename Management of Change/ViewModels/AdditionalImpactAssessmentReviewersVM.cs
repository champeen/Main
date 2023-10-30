using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class AdditionalImpactAssessmentReviewersVM : TimeStamps
    {
        public List<AdditionalImpactAssessmentReviewers> AdditionalImpactAssessmentReviewers { get; set; }
        public int ChangeRequestId { get; set; }
        public string Tab {  get; set; }   
        public bool EquipmentReviewerRequired { get; set; }
        public bool MaintenanceReviewerRequired { get; set; }
        public List<AdditionalImpactAssessmentReviewers> EquipmentReviewers { get; set; }
        public List<AdditionalImpactAssessmentReviewers> MaintenanceReviewers { get; set; }
    }
}
