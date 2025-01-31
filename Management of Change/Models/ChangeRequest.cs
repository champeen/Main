using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Security.AccessControl;

namespace Management_of_Change.Models
{
    public class ChangeRequest : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "MOC #")]
        public string? MOC_Number { get; set; }
        [Display(Name = "Change Owner")]
        public string Change_Owner { get; set; }
        [Display(Name = "Change Owner Full Name")]
        public string? Change_Owner_FullName { get; set; }
        [Display(Name = "Change Owner Email")]
        public string? Change_Owner_Email { get; set; }
        [Display(Name = "Location/Site")]
        public string Location_Site { get; set; }
        [Display(Name = "Title/Change Description")]
        [MaxLength(100)]
        public string Title_Change_Description { get; set; }
        [Display(Name = "Scope of Change")]
        public string Scope_of_the_Change { get; set; }
        [Display(Name = "Justification of Change")]
        public string Justification_of_the_Change { get; set; }
        [Display(Name = "Change Grade")]
        public string Change_Level { get; set; }
        [Display(Name = "Area of Change")]
        public string Area_of_Change { get; set; }
        [Display(Name = "Expiration Date if Temporary")]
        [DataType(DataType.Date)]
        public DateTime? Expiration_Date_Temporary { get; set; }
        [Display(Name = "Status")]
        public string Change_Status { get; set; }
        [Display(Name = "Status")]
        public string Change_Status_Description { get; set; }        
        public string? Priority { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        public DateTime Request_Date { get; set; }
        [Display(Name = "Product Line")]
        public string Proudct_Line { get; set; }
        [Display(Name = "Change Type")]
        public string Change_Type { get; set; }
        [Display(Name = "PTN Number")]
        public List<string>? PTN_Number { get; set; }
        [Display(Name = "Waiver Number")]
        public string? Waiver_Number { get; set; }
        //[Display(Name = "CMT Number")]
        //public string? CMT_Number { get; set; }
        [Display(Name = "Estimated Completion Date")]
        [DataType(DataType.Date)]
        public DateTime? Estimated_Completion_Date { get; set; }
        public String? ChangeGradeApprovalUser { get; set; }
        [Display(Name = "Change Grade Approval Username")]
        public String? ChangeGradeApprovalUserFullName { get; set; }
        [Display(Name = "Change Grade Approval Date")]
        [DataType(DataType.Date)]
        public DateTime? ChangeGradeApprovalDate { get; set; }
        public String? ChangeGradeRejectedUser { get; set; }
        [Display(Name = "Change Grade Rejected Username")]
        public String? ChangeGradeRejectedUserFullName { get; set; }
        [Display(Name = "Change Grade Rejected Date")]
        [DataType(DataType.Date)]
        public DateTime? ChangeGradeRejectedDate { get; set; }
        [Display(Name = "Change Grade Rejected Reason")]
        public String? ChangeGradeRejectedReason { get; set; }
        [Display(Name = "Implementation Approval Date")]
        [DataType(DataType.Date)]
        public DateTime? Implementation_Approval_Date { get; set; }
        [Display(Name = "Implementation Approval Username")]
        public string? Implementation_Username { get; set; }
        [Display(Name = "Closeout Date")]
        [DataType(DataType.Date)]
        public DateTime? Closeout_Date { get; set; }
        [Display(Name = "Closeout Username")]
        public string? Closeout_Username { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Cancel Date")]
        public DateTime? Cancel_Date { get; set; }
        [Display(Name = "Cancel Username")]
        public string? Cancel_Username { get; set; }
        [Display(Name = "Cancel Reason")]
        public string? Cancel_Reason { get; set; }
        [Display(Name = "Raw Materials/Component Numbers Impacted")]
        public string? Raw_Material_Component_Numbers_Impacted {get; set;}
        [Display(Name = "Additional Notification(s) of Change Request")]
        public List<string>? Additional_Notification { get; set; }
        public List<GeneralMocResponses>? GeneralMocResponses { get; set; }
        public List<ImpactAssessmentResponse>? ImpactAssessmentResponses { get; set; }
        public List<ImplementationFinalApprovalResponse>? ImplementationFinalApprovalResponses { get; set; }
        public List<PCCB>? PccbMeetings { get; set; }
    }
}
