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
    [Route("api/v1/[controller]")]
    public sealed class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService) =>
            _walletService = walletService;

        /// <summary>Get all wallets for the current user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WalletDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _walletService.GetAllAsync(GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Get a wallet by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _walletService.GetByIdAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Get aggregated finance summary.</summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(FinanceSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary([FromQuery] string currency = "SAR", CancellationToken ct = default)
        {
            var result = await _walletService.GetSummaryAsync(GetUserId(), currency, ct);
            return Ok(result);
        }

        /// <summary>Create a new wallet.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(WalletDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateWalletDto dto, CancellationToken ct)
        {
            var result = await _walletService.CreateAsync(dto, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update a wallet.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalletDto dto, CancellationToken ct)
        {
            var result = await _walletService.UpdateAsync(id, dto, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Delete a wallet (soft delete).</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _walletService.DeleteAsync(id, GetUserId(), ct);
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
