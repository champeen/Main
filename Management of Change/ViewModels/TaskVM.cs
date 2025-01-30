using Management_of_Change.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class TaskVM 
    {
        public Task Task { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public string? FileAttachmentError { get; set; }

        public string? TabActiveDetail { get; set; }
        public string? TabActiveAttachments { get; set; }
    }
}
