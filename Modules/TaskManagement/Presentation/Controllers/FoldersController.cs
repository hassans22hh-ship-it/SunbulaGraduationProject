using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskPresentation.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public sealed class FoldersController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FoldersController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FolderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _folderService.GetAllByUserIdAsync(GetUserId(), ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FolderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _folderService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}/tasks")]
        [ProducesResponseType(typeof(FolderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdWithTasks(Guid id, CancellationToken ct)
        {
            var result = await _folderService.GetByIdWithTasksAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FolderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFolderDto dto, CancellationToken ct)
        {
            var result = await _folderService.CreateAsync(dto, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(FolderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFolderDto dto, CancellationToken ct)
        {
            var result = await _folderService.UpdateAsync(id, dto, GetUserId(), ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _folderService.DeleteAsync(id, GetUserId(), ct);
            return NoContent();
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedAccessException("User ID not found in token."));
    }
}
