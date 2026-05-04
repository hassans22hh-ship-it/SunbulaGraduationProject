using Application.Services.Abstraction; // IUserIntegrationService
using AutoMapper;
using TimeTrackingApplication.TimeDtos;
using TimeTrackingApplication.TimeServiceAbstraction;
using TimeTrackingDomain.Contracts;

namespace TimeTrackingInfrastructure.TimeServices
{
    public sealed class DailyTransactionService:IDailyTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserIntegrationService _userIntegrationService;

        public DailyTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserIntegrationService userIntegrationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userIntegrationService = userIntegrationService;
        }

        public async Task<DailyTransactionDto?> GetByDateAsync(
            Guid userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var daily = await _unitOfWork.DailyTransactions.GetByUserAndDateAsync(userId, date, cancellationToken);
            return daily == null ? null : _mapper.Map<DailyTransactionDto>(daily);
        }

        public async Task<IEnumerable<DailyTransactionDto>> GetByDateRangeAsync(
            Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        {
            var transactions = await _unitOfWork.DailyTransactions.GetByUserAndDateRangeAsync(userId, from, to, cancellationToken);
            return _mapper.Map<IEnumerable<DailyTransactionDto>>(transactions);
        }

        public async Task<DailySummaryDto> GetDailySummaryAsync(
            Guid userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var daily = await _unitOfWork.DailyTransactions.GetByUserAndDateAsync(userId, date, cancellationToken);
            var sessions = await _unitOfWork.TimeSessions.GetByUserAndDateAsync(userId, date, cancellationToken);
            var streak = await _unitOfWork.DailyTransactions.GetCurrentStreakAsync(userId, cancellationToken);

            var totalMinutes = daily?.TotalMinutes ?? 0;
            var totalCoins = daily?.TotalCoins ?? 0;
            var sessionCount = daily?.SessionCount ?? 0;
            var qualifies = daily?.QualifiesForStreak() ?? false;

            return new DailySummaryDto
            {
                Date = date,
                TotalMinutes = totalMinutes,
                TotalCoins = totalCoins,
                SessionCount = sessionCount,
                UntrackedMinutes = Math.Max(0, 1440 - totalMinutes),
                CurrentStreak = streak,
                QualifiesForStreak = qualifies,
                Sessions = sessions.Select(s => new TimeSessionDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    TaskId = s.TaskId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    DurationMinutes = s.DurationMinutes,
                    CoinsEarned = s.CoinsEarned,
                    BehaviorType = s.BehaviorType,
                    BehaviorTypeName = s.BehaviorType.ToString(),
                    IsActive = s.IsActive,
                    ManuallyAdded = s.ManuallyAdded,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt
                })
            };
        }

        public async Task<int> GetCurrentStreakAsync(
            Guid userId, CancellationToken cancellationToken = default)
            => await _unitOfWork.DailyTransactions.GetCurrentStreakAsync(userId, cancellationToken);

        public async Task<IEnumerable<DailyTransactionDto>> GetLastNDaysAsync(
            Guid userId, int days, CancellationToken cancellationToken = default)
        {
            if (days < 1 || days > 365)
                throw new ArgumentException("Days must be between 1 and 365.", nameof(days));

            var transactions = await _unitOfWork.DailyTransactions.GetLastNDaysAsync(userId, days, cancellationToken);
            return _mapper.Map<IEnumerable<DailyTransactionDto>>(transactions);
        }

        public async Task CheckAndAwardStreakBonusAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var streak = await GetCurrentStreakAsync(userId, cancellationToken);

            int coinsToAward = 0;
            if (streak == 3) coinsToAward = 50;
            else if (streak == 7) coinsToAward = 150;
            else if (streak == 30) coinsToAward = 700;

            if (coinsToAward > 0)
            {
                // AwardStreakMilestoneAsync calls user.AddCoins() internally,
                // which raises CoinBalanceChangedEvent for SSE streaming.
                // No need for a separate CoinsEarnedEvent — single path avoids double-counting.
                await _userIntegrationService.AwardStreakMilestoneAsync(userId, streak, coinsToAward, cancellationToken);
            }
        }
    }
}
