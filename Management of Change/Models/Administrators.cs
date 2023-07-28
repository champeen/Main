using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class Administrators : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
    }
}
