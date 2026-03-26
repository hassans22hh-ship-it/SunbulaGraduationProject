using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using TaskDomain.Contracts;
using TaskDomain.Entities.TaskManagement.Enums;
using TimeTrackingApplication.TimeServiceAbstraction;
using TimeTrackingApplication.TimeDtos;

namespace TaskInfrastructure.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDailyTransactionService _dailyTransactionService;

        public ReportsService(IUnitOfWork unitOfWork, IDailyTransactionService dailyTransactionService)
        {
            _unitOfWork = unitOfWork;
            _dailyTransactionService = dailyTransactionService;
        }

        public async Task<TaskReportDto> GetUserSummaryReportAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var (tasks, totalTaskCount) = await _unitOfWork.Tasks.GetByUserIdAsync(userId, 1, 1000, cancellationToken);
            var taskList = tasks.ToList();

            var last7Days = await _dailyTransactionService.GetLastNDaysAsync(userId, 7, cancellationToken);
            var last7DaysList = last7Days.ToList();

            var report = new TaskReportDto
            {
                TotalTasks = taskList.Count,
                ActiveTasks = taskList.Count(t => t.Status == TaskDomain.Entities.TaskManagement.Enums.TaskStatus.Active && !t.IsArchived),
                CompletedTasks = taskList.Count(t => t.Status == TaskDomain.Entities.TaskManagement.Enums.TaskStatus.Completed),
                ArchivedTasks = taskList.Count(t => t.IsArchived),
                
                TasksByBehavior = taskList
                    .GroupBy(t => t.BehaviorType.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),

                TotalTimeSpentMinutes = last7DaysList.Sum(d => d.TotalMinutes),
                TotalCoinsEarned = last7DaysList.Sum(d => d.TotalCoins),

                Last7DaysProgress = last7DaysList.Select(d => new DailyReportDetailDto
                {
                    Date = d.Date,
                    TasksCompleted = d.SessionCount, // SessionCount is a proxy for task activity
                    MinutesSpent = d.TotalMinutes,
                    CoinsEarned = d.TotalCoins
                }).OrderBy(d => d.Date).ToList()
            };

            return report;
        }
    }
}
