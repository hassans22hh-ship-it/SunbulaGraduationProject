using Application.TaskManagmentDTOS;

namespace Application.ServiceAbstraction
{
    public interface IReportsService
    {
        Task<TaskReportDto> GetUserSummaryReportAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
