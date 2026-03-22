using System.ComponentModel.DataAnnotations;
using TimeTrackingDomain.Enums;

namespace TimeTrackingApplication.TimeDtos
{
    public sealed record StartSessionDto
    {
        [Required(ErrorMessage = "TaskId is required.")]
        public Guid TaskId { get; init; }

        [Required(ErrorMessage = "BehaviorType is required.")]
        public BehaviorType BehaviorType { get; init; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; init; }
    }
}

