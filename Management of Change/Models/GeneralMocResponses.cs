using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class GeneralMocResponses : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        public string? Response { get; set; }
        public string? Order { get; set; }
        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }
        //public ChangeRequest ChangeRequest { get; set; }
    }
}
