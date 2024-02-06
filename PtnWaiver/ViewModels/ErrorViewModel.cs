namespace PtnWaiver.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? ErrorMessage { get; set; }

    }
}