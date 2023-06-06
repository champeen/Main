namespace Management_of_Change.Models
{
    public class TimeStamps
    {
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
