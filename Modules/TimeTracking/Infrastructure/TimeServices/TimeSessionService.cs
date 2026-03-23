using AutoMapper;
using TimeTrackingApplication.TimeDtos;
using TimeTrackingApplication.TimeServiceAbstraction;
using TimeTrackingDomain.Contracts;
using TimeTrackingDomain.Entities;
using TimeTrackingDomain.Exceptions;

namespace TimeTrackingInfrastructure.TimeServices
{
    public class TimeSessionService: ITimeSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TimeSessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TimeSessionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetByIdAsync(id, cancellationToken)
                ?? throw new TimeSessionNotFoundException(id);
            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<IEnumerable<TimeSessionDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.TimeSessions.GetByUserIdAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<TimeSessionDto>>(sessions);
        }

        public async Task<IEnumerable<TimeSessionDto>> GetByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.TimeSessions.GetByUserAndDateAsync(userId, date, cancellationToken);
            return _mapper.Map<IEnumerable<TimeSessionDto>>(sessions);
        }

        public async Task<IEnumerable<TimeSessionDto>> GetByDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        {
            if (from > to) throw new ArgumentException("'From' date must be before or equal to 'To' date.");
            var sessions = await _unitOfWork.TimeSessions.GetByUserAndDateRangeAsync(userId, from, to, cancellationToken);
            return _mapper.Map<IEnumerable<TimeSessionDto>>(sessions);
        }

        public async Task<(IEnumerable<TimeSessionDto> Sessions, int TotalCount)> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;
            var (sessions, total) = await _unitOfWork.TimeSessions.GetPagedByUserIdAsync(userId, page, pageSize, cancellationToken);
            return (_mapper.Map<IEnumerable<TimeSessionDto>>(sessions), total);
        }

        public async Task<TimeSessionDto?> GetActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetActiveSessionByUserIdAsync(userId, cancellationToken);
            return session == null ? null : _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<TimeSessionDto> StartAsync(StartSessionDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var activeSession = await _unitOfWork.TimeSessions.GetActiveSessionByUserIdAsync(userId, cancellationToken);
            if (activeSession != null)
                throw new ActiveSessionExistsException(activeSession.Id);

            var session = TimeSession.Start(userId, dto.TaskId, dto.BehaviorType, dto.Notes);
            await _unitOfWork.TimeSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<TimeSessionDto> StopAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetByIdAsync(sessionId, cancellationToken)
                ?? throw new TimeSessionNotFoundException(sessionId);

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to stop this session.");

            if (!session.IsActive)
            {
                return _mapper.Map<TimeSessionDto>(session);
            }

            session.Stop();
            await UpdateDailyTransactionAsync(session, cancellationToken);
            _unitOfWork.TimeSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<TimeSessionDto?> StopActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetActiveSessionByUserIdAsync(userId, cancellationToken);
            if (session == null) return null;

            session.Stop();
            await UpdateDailyTransactionAsync(session, cancellationToken);
            _unitOfWork.TimeSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<TimeSessionDto> CreateManualAsync(CreateTimeSessionDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var overlapping = await _unitOfWork.TimeSessions.GetOverlappingSessionsAsync(userId, dto.StartTime, dto.EndTime, cancellationToken: cancellationToken);
            if (overlapping.Any()) throw new OverlappingSessionException(dto.StartTime, dto.EndTime);

            var session = TimeSession.CreateManual(userId, dto.TaskId, dto.StartTime, dto.EndTime, dto.BehaviorType, dto.Notes);
            await UpdateDailyTransactionAsync(session, cancellationToken);
            await _unitOfWork.TimeSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task<TimeSessionDto> UpdateAsync(Guid id, UpdateTimeSessionDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetByIdAsync(id, cancellationToken)
                ?? throw new TimeSessionNotFoundException(id);

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this session.");

            var overlapping = await _unitOfWork.TimeSessions.GetOverlappingSessionsAsync(userId, dto.StartTime, dto.EndTime, id, cancellationToken);
            if (overlapping.Any()) throw new OverlappingSessionException(dto.StartTime, dto.EndTime);

            var oldDuration = session.DurationMinutes;
            var oldCoins = session.CoinsEarned;

            session.Update(dto.StartTime, dto.EndTime, dto.BehaviorType, dto.Notes);

            var date = DateOnly.FromDateTime(session.StartTime);
            var daily = await GetOrCreateDailyTransactionAsync(userId, date, cancellationToken);
            daily.UpdateSession(oldDuration, oldCoins, session.DurationMinutes, session.CoinsEarned);

            _unitOfWork.TimeSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TimeSessionDto>(session);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetByIdAsync(id, cancellationToken)
                ?? throw new TimeSessionNotFoundException(id);

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this session.");

            if (!session.IsActive && session.EndTime.HasValue)
            {
                var date = DateOnly.FromDateTime(session.StartTime);
                var daily = await _unitOfWork.DailyTransactions.GetByUserAndDateAsync(userId, date, cancellationToken);
                daily?.RemoveSession(session.DurationMinutes, session.CoinsEarned);
            }

            _unitOfWork.TimeSessions.Delete(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<TimeSessionDto> RecoverSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TimeSessions.GetByIdAsync(sessionId, cancellationToken)
                ?? throw new TimeSessionNotFoundException(sessionId);

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to recover this session.");

            session.RecoverFromDisconnect(DateTime.UtcNow);
            await UpdateDailyTransactionAsync(session, cancellationToken);
            _unitOfWork.TimeSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TimeSessionDto>(session);
        }

        private async Task UpdateDailyTransactionAsync(TimeSession session, CancellationToken cancellationToken)
        {
            var date = DateOnly.FromDateTime(session.StartTime);
            var daily = await GetOrCreateDailyTransactionAsync(session.UserId, date, cancellationToken);
            daily.AddSession(session.DurationMinutes, session.CoinsEarned);
        }

        private async Task<DailyTransaction> GetOrCreateDailyTransactionAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
        {
            var daily = await _unitOfWork.DailyTransactions.GetByUserAndDateAsync(userId, date, cancellationToken);
            if (daily != null) return daily;
            daily = DailyTransaction.Create(userId, date);
            await _unitOfWork.DailyTransactions.AddAsync(daily, cancellationToken);
            return daily;
        }
    }
}

