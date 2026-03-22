using AutoMapper;
using TimeTrackingApplication.TimeDtos;
using TimeTrackingDomain.Entities;

namespace TimeTrackingApplication.Mappings
{
    /// AutoMapper profile for TimeTracking module.
    /// Maps Domain entities → Application DTOs.
    public class TimeTrackingMappingProfile: Profile
    {
        public TimeTrackingMappingProfile()
        {
            // ── TimeSession ──────────────────────────────────────────────

            CreateMap<TimeSession, TimeSessionDto>()
                .ForMember(dest => dest.BehaviorTypeName,
                           opt => opt.MapFrom(src => src.BehaviorType.ToString()))
                .ForMember(dest => dest.FormattedDuration,
                           opt => opt.Ignore()); // Computed property — no mapping needed

            // ── DailyTransaction ────────────────────────────────────────

            CreateMap<DailyTransaction, DailyTransactionDto>()
                .ForMember(dest => dest.QualifiesForStreak,
                           opt => opt.MapFrom(src => src.QualifiesForStreak()))
                .ForMember(dest => dest.FormattedTotalTime,
                           opt => opt.Ignore())
                .ForMember(dest => dest.UntrackedMinutes,
                           opt => opt.Ignore()); // Computed — handled in record
        }
    }
}

