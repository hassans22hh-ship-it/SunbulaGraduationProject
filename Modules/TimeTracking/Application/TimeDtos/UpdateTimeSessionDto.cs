using System.ComponentModel.DataAnnotations;
using TimeTrackingDomain.Enums;

namespace TimeTrackingApplication.TimeDtos
{
    public sealed record UpdateTimeSessionDto
    {
        [Required(ErrorMessage = "StartTime is required.")]
        public DateTime StartTime { get; init; }

        [Required(ErrorMessage = "EndTime is required.")]
        public DateTime EndTime { get; init; }

        [Required(ErrorMessage = "BehaviorType is required.")]
        public BehaviorType BehaviorType { get; init; }

        [MaxLength(500)]
        public string? Notes { get; init; }
    }
}

