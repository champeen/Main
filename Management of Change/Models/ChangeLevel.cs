namespace Management_of_Change.Models
{
    public class ChangeLevel : TimeStamps
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string? Description { get; set; }
    }
}
