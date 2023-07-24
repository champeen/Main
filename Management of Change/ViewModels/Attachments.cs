using Management_of_Change.Models;
using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.ViewModels
{
    public class Attachment
    {
        public string? Directory { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public string? FullPath { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        public int Size { get; set; }
    }
}
