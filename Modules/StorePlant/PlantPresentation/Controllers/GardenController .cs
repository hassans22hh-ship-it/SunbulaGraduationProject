using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantApplication.StorePlantDTOs;
using PlantApplication.StorePlantServiceAbstraction;
using System.Security.Claims;

namespace PlantPresentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/garden")]
    public sealed class GardenController : ControllerBase
    {
        private readonly IUserPlantService _userPlantService;

        public GardenController(IUserPlantService userPlantService)
        {
            _userPlantService = userPlantService;
        }

        /// <summary>Get the current user's full garden summary with statistics.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(GardenSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGarden(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userPlantService.GetGardenAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>Get a specific owned plant with growth history.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserPlantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userPlantService.GetByIdAsync(id, userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Purchase a plant from the virtual store.
        /// Coin balance is passed from the caller (resolved via UserIdentity integration).
        /// On success, a PlantPurchasedEvent is raised for UserIdentity to deduct coins.
        /// </summary>
        [HttpPost("purchase")]
        [ProducesResponseType(typeof(UserPlantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Purchase(
            [FromBody] PurchasePlantDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userPlantService.PurchasePlantAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Trigger a growth coin event for a plant.
        /// Called internally by TimeTracking integration when coins are earned.
        /// </summary>
        [HttpPost("{id:guid}/grow")]
        [ProducesResponseType(typeof(UserPlantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddGrowthCoins(Guid id, [FromQuery] int coins, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userPlantService.AddGrowthCoinsAsync(id, userId, coins, cancellationToken);
            return Ok(result);
        }

        // ── Private Helpers ────────────────────────────────────────────────

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in JWT token.");
            return Guid.Parse(value);
        }
    }
}
