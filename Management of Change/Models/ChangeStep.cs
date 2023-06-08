namespace Management_of_Change.Models
{
    public class ChangeStep : TimeStamps
    {
        public int Id { get; set; }
        public string Step { get; set; }
        public string? Order { get; set; }
    }
}
