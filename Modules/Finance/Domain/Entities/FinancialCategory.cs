using SharedKernel;

namespace FinanceDomain.Entities
{
    /// FinancialCategory entity.
    /// User-defined categories for organizing transactions (e.g., Food, Transportation, Salary).
    public class FinancialCategory:BaseEntity
    {
        private readonly List<FinancialTransaction> _transactions = new();

        private FinancialCategory() { }

        private FinancialCategory(Guid id, Guid userId, string name, string? icon) : base(id)
        {
            UserId = userId;
            Name = name;
            Icon = icon;
        }

        /// <summary>Owner user ID (cross-module reference).</summary>
        public Guid UserId { get; private set; }

        /// <summary>Category name (e.g., "Food", "Rent", "Salary").</summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>Optional emoji icon for the category (e.g., "🍔", "🏠", "💰").</summary>
        public string? Icon { get; private set; }

        /// <summary>Transactions using this category.</summary>
        public IReadOnlyCollection<FinancialTransaction> Transactions => _transactions.AsReadOnly();

        // ─── Factory ────────────────────────────────────────────────────────────

        /// <summary>Creates a new financial category for a user.</summary>
        public static FinancialCategory Create(Guid userId, string name, string? icon = null)
        {
            ValidateName(name);
            return new FinancialCategory(Guid.NewGuid(), userId, name, icon);
        }

        // ─── Domain Methods ─────────────────────────────────────────────────────

        /// <summary>Renames the category.</summary>
        public void Rename(string newName)
        {
            ValidateName(newName);
            Name = newName;
            MarkAsUpdated();
        }

        /// <summary>Updates the emoji icon for this category.</summary>
        public void UpdateIcon(string? icon)
        {
            Icon = icon;
            MarkAsUpdated();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.", nameof(name));

            if (name.Length < 2)
                throw new ArgumentException("Category name must be at least 2 characters.", nameof(name));

            if (name.Length > 50)
                throw new ArgumentException("Category name cannot exceed 50 characters.", nameof(name));
        }
    }
}
