using FinanceApplication.financedtos;
using FinanceApplication.FinanceServiceAbs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancePresentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/financial-categories")]
    public sealed class FinancialCategoriesController : ControllerBase
    {
        private readonly IFinancialCategoryService _categoryService;

        public FinancialCategoriesController(IFinancialCategoryService categoryService) =>
            _categoryService = categoryService;

        /// <summary>Get all financial categories for the current user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FinancialCategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _categoryService.GetAllAsync(GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Get a financial category by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FinancialCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Create a new financial category.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(FinancialCategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFinancialCategoryDto dto, CancellationToken ct)
        {
            var result = await _categoryService.CreateAsync(dto, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Rename a financial category.</summary>
        [HttpPut("{id:guid}/rename")]
        [ProducesResponseType(typeof(FinancialCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Rename(Guid id, [FromQuery] string newName, CancellationToken ct)
        {
            var result = await _categoryService.RenameAsync(id, newName, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Delete a financial category (soft delete).</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _categoryService.DeleteAsync(id, GetUserId(), ct);
            return NoContent();
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token."));
    }
}
