namespace Management_of_Change.Models
{
    public class ChangeType : TimeStamps
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
    }
}
