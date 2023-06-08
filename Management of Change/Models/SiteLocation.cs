namespace Management_of_Change.Models
{
    public class SiteLocation : TimeStamps
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string? Order { get; set; }
    }
}
