using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrackingApplication.TimeDtos;
using TimeTrackingApplication.TimeServiceAbstraction;

namespace TimeTrackingPresentation.Controllers
{
    [Authorize]
    /// Provides daily summaries, timeline data, and streak information.
    [Route("api/[controller]")]
    [ApiController]
    public class DailyTransactionController: ControllerBase
    {
        private readonly IDailyTransactionService _dailyService;

        public DailyTransactionController(IDailyTransactionService dailyService)
        {
            _dailyService = dailyService;
        }

        [HttpGet("{date}/summary")]
        [ProducesResponseType(typeof(DailySummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary(DateOnly date, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _dailyService.GetDailySummaryAsync(userId, date, cancellationToken);
            return Ok(result);
        }

        [HttpGet("today/summary")]
        [ProducesResponseType(typeof(DailySummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodaySummary(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var result = await _dailyService.GetDailySummaryAsync(userId, today, cancellationToken);
            return Ok(result);
        }

        [HttpGet("range")]
        [ProducesResponseType(typeof(IEnumerable<DailyTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRange([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var result = await _dailyService.GetByDateRangeAsync(userId, from, to, cancellationToken);
            return Ok(result);
        }

        [HttpGet("last/{days:int}")]
        [ProducesResponseType(typeof(IEnumerable<DailyTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLastNDays(int days, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _dailyService.GetLastNDaysAsync(userId, days, cancellationToken);
            return Ok(result);
        }

        [HttpGet("streak")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStreak(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var streak = await _dailyService.GetCurrentStreakAsync(userId, cancellationToken);
            return Ok(new { CurrentStreak = streak });
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token.");
            return Guid.Parse(claim);
        }
    }
}

