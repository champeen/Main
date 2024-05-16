using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class PccbInvitees : TimeStamps
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string? Title { get; set; }
        public bool? Attended { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        [ForeignKey("PCCB")]
        public int PccbId { get; set; }
        public int MocId { get; set; }
    }
}
