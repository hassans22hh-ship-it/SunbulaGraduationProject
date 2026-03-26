using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskDomain.Entities.TaskManagement.Enums;

namespace TaskPresentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public sealed class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetAllByUserIdAsync(GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive([FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetActiveByUserIdAsync(GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("archived")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchived([FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetArchivedByUserIdAsync(GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _taskService.GetByIdAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}/details")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdWithDetails(Guid id, CancellationToken ct)
        {
            var result = await _taskService.GetByIdWithDetailsAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("folder/{folderId:guid}")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFolder(Guid folderId, [FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetByFolderIdAsync(folderId, GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("category/{categoryId:guid}")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(Guid categoryId, [FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetByCategoryIdAsync(categoryId, GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("behavior/{behaviorType}")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBehavior(BehaviorCategory behaviorType, [FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.GetByBehaviorTypeAsync(GetUserId(), behaviorType, pagination, ct);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResultDto<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] PaginationParams pagination, CancellationToken ct)
        {
            var result = await _taskService.SearchAsync(query ?? string.Empty, GetUserId(), pagination, ct);
            return Ok(result);
        }

        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecent(CancellationToken ct)
        {
            var result = await _taskService.GetRecentAsync(GetUserId(), 10, ct);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
        {
            var result = await _taskService.CreateAsync(dto, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id:guid}/duplicate")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Duplicate(Guid id, CancellationToken ct)
        {
            var result = await _taskService.DuplicateAsync(id, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
        {
            var result = await _taskService.UpdateAsync(id, dto, GetUserId(), ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/archive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            await _taskService.ArchiveAsync(id, GetUserId(), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/unarchive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unarchive(Guid id, CancellationToken ct)
        {
            await _taskService.UnarchiveAsync(id, GetUserId(), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
        {
            await _taskService.CompleteAsync(id, GetUserId(), ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _taskService.DeleteAsync(id, GetUserId(), ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/categories/{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddCategory(Guid id, Guid categoryId, CancellationToken ct)
        {
            await _taskService.AddCategoryAsync(id, categoryId, GetUserId(), ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}/categories/{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCategory(Guid id, Guid categoryId, CancellationToken ct)
        {
            await _taskService.RemoveCategoryAsync(id, categoryId, GetUserId(), ct);
            return NoContent();
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");
            if (!Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID format in token.");
            return userId;
        }
    }
}
