using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DebtApplication.Dtos;
using DebtApplication.DebtService;
using Microsoft.AspNetCore.Authorization;
using DebtDomain.Enums;
namespace DebtPresentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public sealed class DebtController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        /// <summary>
        /// Gets all debts for the current user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DebtDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.GetAllByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a debt by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DebtDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _debtService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a debt with payment history.
        /// </summary>
        [HttpGet("{id:guid}/with-payments")]
        [ProducesResponseType(typeof(DebtWithPaymentsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdWithPayments(Guid id, CancellationToken cancellationToken)
        {
            var result = await _debtService.GetByIdWithPaymentsAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets all unpaid debts for the current user.
        /// </summary>
        [HttpGet("unpaid")]
        [ProducesResponseType(typeof(IEnumerable<DebtDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnpaid(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.GetUnpaidByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets all overdue debts for the current user.
        /// </summary>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<DebtDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOverdue(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.GetOverdueByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets debts by type (Payable or Receivable).
        /// </summary>
        [HttpGet("by-type/{debtType}")]
        [ProducesResponseType(typeof(IEnumerable<DebtDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByType(DebtType debtType, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.GetByTypeAsync(userId, debtType, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets debt summary statistics for the current user.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(DebtSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.GetDebtSummaryAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new debt.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DebtDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateDebtDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.CreateAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates a debt.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DebtDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateDebtDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.UpdateAsync(id, dto, userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Records a payment against a debt.
        /// </summary>
        [HttpPost("{id:guid}/payments")]
        [ProducesResponseType(typeof(DebtPaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RecordPayment(
            Guid id,
            [FromBody] RecordPaymentDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _debtService.RecordPaymentAsync(id, dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, result);
        }

        /// <summary>
        /// Marks a debt as fully paid.
        /// </summary>
        [HttpPost("{id:guid}/mark-paid")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsPaid(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _debtService.MarkAsPaidAsync(id, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Reopens a paid debt.
        /// </summary>
        [HttpPost("{id:guid}/reopen")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reopen(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _debtService.ReopenAsync(id, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a debt.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _debtService.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
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

