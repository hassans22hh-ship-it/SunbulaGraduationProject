using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using TaskDomain.Entities.TaskManagement.Enums;

namespace TaskManagement.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetAllByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetActiveByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("archived")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchived(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetArchivedByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-behavior/{behaviorType}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBehaviorType(BehaviorCategory behaviorType, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetByBehaviorTypeAsync(userId, behaviorType, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _taskService.GetByIdWithDetailsAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTaskDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.CreateAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateTaskDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.UpdateAsync(id, dto, userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/archive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.ArchiveAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/unarchive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unarchive(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.UnarchiveAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.CompleteAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{taskId:guid}/categories/{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddCategory(Guid taskId, Guid categoryId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.AddCategoryAsync(taskId, categoryId, userId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{taskId:guid}/categories/{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCategory(Guid taskId, Guid categoryId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _taskService.RemoveCategoryAsync(taskId, categoryId, userId, cancellationToken);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token");
            return Guid.Parse(userIdClaim);
        }
    }
}
