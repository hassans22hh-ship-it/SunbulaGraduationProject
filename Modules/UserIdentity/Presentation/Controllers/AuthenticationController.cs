using System.Security.Claims;
using Application.UserDTO;
using Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace PresentationIdentity.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthREsponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto,
        CancellationToken cancellationToken)
        {
            var result = await _authenticationService.RegisterAsync(registerDto, cancellationToken);
            return CreatedAtAction(nameof(GetProfile), new { }, result);
        }
        /// <summary>
        /// Login with email and password.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthREsponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto,
            CancellationToken cancellationToken)
        {
            var deviceInfo = HttpContext.Request.Headers["User-Agent"].ToString();
            var result = await _authenticationService.LoginAsync(loginDto, deviceInfo, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Refresh access token using refresh token.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthREsponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenDto request,
          string? deviceInfo = null, CancellationToken cancellationToken = default)
        {
            var result = await _authenticationService.RefreshTokenAsync(request.RefreshToken, deviceInfo, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Logout from current device or all devices.
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(
            [FromBody] RefreshTokenDto? request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _authenticationService.LogoutAsync(userId, request?.RefreshToken, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Get current user profile.
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _authenticationService.GetUserProfileAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Confirm email address (Redirects to frontend).
        /// </summary>
        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            await _authenticationService.ConfirmEmailAsync(token, cancellationToken);

            // Redirect to frontend dashboard or a success page
            return Redirect("https://sunbula-front-end-474s.vercel.app/tasks");
        }

        /// <summary>
        /// Update user profile.
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _authenticationService.UpdateProfileAsync(userId, dto, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _authenticationService.ChangePasswordAsync(userId, dto, cancellationToken);
            return Ok(new { message = "Password changed successfully." });
        }

        /// <summary>
        /// Deletes the user account and all associated data.
        /// </summary>
        [Authorize]
        [HttpDelete("account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAccount(
            [FromBody] DeleteAccountDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _authenticationService.DeleteAccountAsync(userId, dto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Resets the user's coin balance to zero.
        /// </summary>
        [Authorize]
        [HttpPost("reset-coins")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetCoins(
            [FromBody] ResetCoinsDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _authenticationService.ResetCoinsAsync(userId, dto, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("resend-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResendConfirmation(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _authenticationService.ResendConfirmationEmailAsync(userId, cancellationToken);
            return Ok(new { message = "Confirmation email resent." });
        }

        /// <summary>
        /// Listen to real-time changes in the user's coin balance using Server-Sent Events (SSE).
        /// </summary>
        [Authorize]
        [HttpGet("coins/listen")]
        [Produces("text/event-stream")]
        public async Task ListenToCoins(
            [FromServices] ICoinStreamManager coinStreamManager,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            // Flush headers to establish SSE connection
            await Response.Body.FlushAsync(cancellationToken);

            var channel = coinStreamManager.Subscribe(userId);

            try
            {
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };

                await foreach (var coinEvent in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    var payload = new
                    {
                        coinEvent.NewBalance,
                        coinEvent.Change,
                        coinEvent.Reason,
                        coinEvent.PreviousBalance,
                        coinEvent.UserId
                    };

                    var dataLine = $"data: {System.Text.Json.JsonSerializer.Serialize(payload, jsonOptions)}\n\n";
                    await Response.WriteAsync(dataLine, cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected
            }
            finally
            {
                coinStreamManager.Unsubscribe(userId, channel);
            }
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
