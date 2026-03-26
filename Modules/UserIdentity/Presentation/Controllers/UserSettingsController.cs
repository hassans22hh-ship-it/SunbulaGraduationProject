using System.Security.Claims;
using Application.UserDTO;
using Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PresentationIdentity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsService _userSettingsService;

        public UserSettingsController(IUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        /// <summary>
        /// Gets the current user's settings.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userSettingsService.GetSettingsAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Updates the current user's settings.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSettings(
            [FromBody] UpdateUserSettingsDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _userSettingsService.UpdateSettingsAsync(userId, dto, cancellationToken);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token");
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID format in token.");
            return userId;
        }
    }
}
