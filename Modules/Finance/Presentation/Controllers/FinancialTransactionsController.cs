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
    [Route("api/transactions")]
    public sealed class FinancialTransactionsController : ControllerBase
    {
        private readonly IFinancialTransactionService _txService;

        public FinancialTransactionsController(IFinancialTransactionService txService) =>
            _txService = txService;

        /// <summary>Get all transactions for the current user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FinancialTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _txService.GetAllAsync(GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Get transactions by date range.</summary>
        [HttpGet("range")]
        [ProducesResponseType(typeof(IEnumerable<FinancialTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken ct)
        {
            var result = await _txService.GetByDateRangeAsync(GetUserId(), from, to, ct);
            return Ok(result);
        }

        /// <summary>Get transactions for a specific wallet.</summary>
        [HttpGet("wallet/{walletId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<FinancialTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByWallet(Guid walletId, CancellationToken ct)
        {
            var result = await _txService.GetByWalletAsync(walletId, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Get a transaction by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FinancialTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _txService.GetByIdAsync(id, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Record a new financial transaction (Income / Expense / Transfer).</summary>
        [HttpPost]
        [ProducesResponseType(typeof(FinancialTransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateFinancialTransactionDto dto, CancellationToken ct)
        {
            var result = await _txService.CreateAsync(dto, GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update a transaction (amount, category, description, date).</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(FinancialTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateFinancialTransactionDto dto, CancellationToken ct)
        {
            var result = await _txService.UpdateAsync(id, dto, GetUserId(), ct);
            return Ok(result);
        }

        /// <summary>Delete (undo) a transaction — reverses wallet balance automatically.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _txService.DeleteAsync(id, GetUserId(), ct);
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
