using Management_of_Change.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class PCCB : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [Display(Name = "Meeting Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime? MeetingDateTime { get; set; }
        public string? Step { get; set; }
        public string? Agenda { get; set; }
        public string? Decisions { get; set; }
        [Display(Name = "Action Items")]
        public string? ActionItems { get; set; }         
        public string? Notes { get; set; }
        public string Status { get; set; }
        [Display(Name = "Invitee List")]
        public string? InviteeList { get; set; }
        [Display(Name = "Notification List")]
        public string? NotificationList { get; set; }
        public List<PccbInvitees>? Invitees { get; set; }     

        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }

        
    }
}
