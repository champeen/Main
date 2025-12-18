using EHS.Models;
using EHS.Models.IH;
using System.ComponentModel.DataAnnotations;

namespace EHS.ViewModels
{
    public class ChemicalRiskAssessmentViewModel
    {
        public chemical_risk_assessment chemical_risk_assessment { get; set; }
        public List<Attachment>? attachments { get; set; }
        public string? FileAttachmentError { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }
    }
}
