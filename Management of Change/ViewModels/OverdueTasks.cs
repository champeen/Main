using Management_of_Change.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class OverdueTasks 
    {
        public string? UserName { get; set; }
        public string? UserDisplayName { get; set; }
        public int Count { get; set; }
    }
}
