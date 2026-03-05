using System.ComponentModel.DataAnnotations;

namespace FinanceApplication.financedtos
{

    public sealed record CreateFinancialCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; init; } = string.Empty;
    }
}
