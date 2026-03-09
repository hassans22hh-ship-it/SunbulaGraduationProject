using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantApplication.StorePlantDTOs;
using PlantApplication.StorePlantServiceAbstraction;
using PlantDomain.Enums;

namespace PlantPresentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Plant")]
    public sealed class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        /// <summary>Get all available plants in the store.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _plantService.GetAllAvailableAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>Get plants filtered by level (Beginner, Medium, Advanced, Rare).</summary>
        [HttpGet("level/{level}")]
        [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLevel(PlantLevel level, CancellationToken cancellationToken)
        {
            var result = await _plantService.GetByLevelAsync(level, cancellationToken);
            return Ok(result);
        }

        /// <summary>Get currently active seasonal plants.</summary>
        [HttpGet("seasonal")]
        [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeasonal(CancellationToken cancellationToken)
        {
            var result = await _plantService.GetSeasonalPlantsAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>Get a single plant by ID (detail page).</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _plantService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>Add a new plant to the store catalog (Admin).</summary>
        [HttpPost]
        [ProducesResponseType(typeof(PlantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePlantDto dto, CancellationToken cancellationToken)
        {
            var result = await _plantService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update plant store information (Admin).</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlantDto dto, CancellationToken cancellationToken)
        {
            var result = await _plantService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        /// <summary>Toggle plant availability in the store (Admin).</summary>
        [HttpPatch("{id:guid}/availability")]
        [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetAvailability(Guid id, [FromQuery] bool isAvailable, CancellationToken cancellationToken)
        {
            var result = await _plantService.SetAvailabilityAsync(id, isAvailable, cancellationToken);
            return Ok(result);
        }

        /// <summary>Soft-delete a plant from the store (Admin).</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _plantService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
