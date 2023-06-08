using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace Management_of_Change.Models
{
    public class ChangeRequest : TimeStamps
    {
        public int Id { get; set; }
        [Display(Name = "MOC #")]
        public string? MOC_Number { get; set; }
        [Display(Name = "Change Owner")]
        public string Change_Owner { get; set; }
        [Display(Name = "Location Site")]
        public string Location_Site { get; set; }
        [Display(Name = "Title/Change Description")]
        public string Title_Change_Description { get; set; }
        [Display(Name = "Scope of Change")]
        public string Scope_of_the_Change { get; set; }
        [Display(Name = "Justification of Change")]
        public string Justification_of_the_Change { get; set; }
        [Display(Name = "Change Level")]
        public string Change_Level { get; set; }
        [Display(Name = "Area of Change")]
        public string Area_of_Change { get; set; }
        [Display(Name = "Expiration Date if Temporary")]
        [DataType(DataType.Date)]
        public DateTime? Expiration_Date_Temporary { get; set; }
        [Display(Name = "Status")]
        public string Change_Status { get; set; }
        [Display(Name = "Request Date")]
        public DateTime Request_Date { get; set; }
        [Display(Name = "Product Line")]
        public string Proudct_Line { get; set; }
        [Display(Name = "Change Type")]
        public string Change_Type { get; set; }
        [Display(Name = "Estimated Completion Date")]
        [DataType(DataType.Date)]
        public DateTime Estimated_Completion_Date { get; set; }
        [Display(Name = "Raw Materials/Component Numbers Impacted")]
        public string Raw_Material_Component_Numbers_Impacted {get; set;}
    }
}
