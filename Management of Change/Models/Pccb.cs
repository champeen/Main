using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class PCCB : TimeStamps
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Display(Name = "Meeting Date")]
        [DataType(DataType.Date)]
        public DateTime? MeetingDate { get; set; }
        [Display(Name = "Meeting Time")]
        [DataType(DataType.Time)]
        public DateTime? MeetingTime { get; set; }
        [Display(Name = "Meeting Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime? MeetingDateTime { get; set; }
        public string? Agenda { get; set; }
        public string? Decisions { get; set; }
        [Display(Name = "Action Items")]
        public string? ActionItems { get; set; }
        public string Status { get; set; }  // Scheduled/Closed

        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }

        public List<PccbInvitees>? Invitees { get; set; }
    }
}
