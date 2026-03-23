using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrackingApplication.TimeDtos;
using TimeTrackingApplication.TimeServiceAbstraction;

namespace TimeTrackingPresentation.Controllers
{
    [Authorize]
    /// Manages time tracking sessions — start, stop, manual entry, and timeline queries.
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSessionController: ControllerBase
    {
        private readonly ITimeSessionService _sessionService;

        public TimeSessionController(ITimeSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TimeSessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var (sessions, total) = await _sessionService.GetPagedAsync(userId, page, pageSize, cancellationToken);
            return Ok(new { Sessions = sessions, TotalCount = total, Page = page, PageSize = pageSize });
        }

        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(IEnumerable<TimeSessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDate(DateOnly date, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.GetByDateAsync(userId, date, cancellationToken);
            return Ok(result);
        }

        [HttpGet("range")]
        [ProducesResponseType(typeof(IEnumerable<TimeSessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.GetByDateRangeAsync(userId, from, to, cancellationToken);
            return Ok(result);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var session = await _sessionService.GetActiveSessionAsync(userId, cancellationToken);
            return session == null ? NoContent() : Ok(session);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("start")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Start([FromBody] StartSessionDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.StartAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id:guid}/stop")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Stop(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.StopAsync(id, userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("stop-active")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> StopActive(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.StopActiveSessionAsync(userId, cancellationToken);
            return result == null ? NoContent() : Ok(result);
        }

        [HttpPost("manual")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateManual([FromBody] CreateTimeSessionDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.CreateManualAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeSessionDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.UpdateAsync(id, dto, userId, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _sessionService.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/recover")]
        [ProducesResponseType(typeof(TimeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Recover(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _sessionService.RecoverSessionAsync(id, userId, cancellationToken);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token.");
            if (!Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID format in token.");
            return userId;
        }
    }
}

