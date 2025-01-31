using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    [Keyless]
    public class __mst_employee 
    {
        [StringLength(150)]
        [Display(Name = "Display Name")]
        public string? displayname { get; set; }
        [StringLength(150)]
        [Display(Name = "First Name")]
        public string? givenname { get; set; }
        [StringLength(150)]
        [Display(Name = "Middle Initial")]
        public string? initials { get; set; }
        [StringLength(150)]
        [Display(Name = "Last Name")]
        public string? surname { get; set; }
        [StringLength(150)]
        [Display(Name = "Job Title")]
        public string? jobtitle { get; set; }
        [StringLength(150)]
        [Display(Name = "Email")]
        public string? mail { get; set; }
        [StringLength(150)]
        [Display(Name = "User Principal Name")]
        public string? userprincipalname { get; set; }
        [Display(Name = "Account Enabled")]
        public bool? accountenabled { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime? createddatetime { get; set; }
        [StringLength(150)]
        [Display(Name = "On-Premise Domain Name")]
        public string? onpremisesdomainname { get; set; }
        [StringLength(150)]
        [Display(Name = "On-Premise Account Name")]
        public string? onpremisessamaccountname { get; set; }
        [Display(Name = "On-Premise Sync Enabled")]
        public bool? onpremisessyncenabled { get; set; }
        [StringLength(150)]
        [Display(Name = "Manager")]
        public string? manager { get; set; }
    }
}
