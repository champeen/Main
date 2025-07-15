using EHS.Models;
using System.ComponentModel.DataAnnotations;

namespace EHS.ViewModels
{
    public class SegViewModel
    {
        public seg_risk_assessment seg_risk_assessment { get; set; }
        public List<Attachment>? attachments { get; set; }
        public string? FileAttachmentError { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }

        // TimeSpan inputs (nullable and validated)
        [Range(0, int.MaxValue)]
        public int? DurationDays { get; set; }

        [Range(0, 23)]
        public int? DurationHours { get; set; }

        [Range(0, 59)]
        public int? DurationMinutes { get; set; }

        [Range(0, 59)]
        public int? DurationSeconds { get; set; }

        // Helper to combine into TimeSpan
        public TimeSpan? DurationOfTask
        {
            get
            {
                if (!DurationDays.HasValue && !DurationHours.HasValue &&
                    !DurationMinutes.HasValue && !DurationSeconds.HasValue)
                    return null;

                return new TimeSpan(
                    DurationDays ?? 0,
                    DurationHours ?? 0,
                    DurationMinutes ?? 0,
                    DurationSeconds ?? 0
                );
            }
        }
    }
}
