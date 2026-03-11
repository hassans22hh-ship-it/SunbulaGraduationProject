using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskDomain.Entities.TaskManagement.Enums;

namespace TaskPresentation.Controllers
{
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
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _taskService.GetAllByUserIdAsync(GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(CancellationToken ct)
        {
            var result = await _taskService.GetActiveByUserIdAsync(GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("archived")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchived(CancellationToken ct)
        {
            var result = await _taskService.GetArchivedByUserIdAsync(GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _taskService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}/details")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdWithDetails(Guid id, CancellationToken ct)
        {
            var result = await _taskService.GetByIdWithDetailsAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("folder/{folderId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFolder(Guid folderId, CancellationToken ct)
        {
            var result = await _taskService.GetByFolderIdAsync(folderId, GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("category/{categoryId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(Guid categoryId, CancellationToken ct)
        {
            var result = await _taskService.GetByCategoryIdAsync(categoryId, GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("behavior/{behaviorType}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBehavior(BehaviorCategory behaviorType, CancellationToken ct)
        {
            var result = await _taskService.GetByBehaviorTypeAsync(GetUserId(), behaviorType, ct);
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

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token."));
    }
}
