using PtnWaiver.Models;
using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.ViewModels
{
    public class Results
    {
        public string id { get; set; }
        public string? text { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}