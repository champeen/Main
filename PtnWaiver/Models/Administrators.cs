using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class Administrators : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Approver { get; set; }
    }
}
