using System.ComponentModel.DataAnnotations;

namespace DebtApplication.Dtos
{
    /// DTO for recording a payment.

    public sealed record RecordPaymentDto
    {
        [Required(ErrorMessage = "Payment amount is required")]
        [Range(0.01, 999999999.99, ErrorMessage = "Payment amount must be between 0.01 and 999,999,999.99")]
        public decimal Amount { get; init; }

        [Required(ErrorMessage = "Payment date is required")]
        public DateTime PaymentDate { get; init; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; init; }
    }
}

