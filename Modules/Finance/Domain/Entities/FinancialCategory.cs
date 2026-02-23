using SharedKernel;

namespace FinanceDomain.Entities
{
    /// FinancialCategory entity.
    /// User-defined categories for organizing transactions (e.g., Food, Transportation, Salary).
    public class FinancialCategory:BaseEntity
    {
        private readonly List<FinancialTransaction> _transactions = new();

        private FinancialCategory() { }

        private FinancialCategory(Guid id, Guid userId, string name) : base(id)
        {
            UserId = userId;
            Name = name;
        }

        /// <summary>Owner user ID (cross-module reference).</summary>
        public Guid UserId { get; private set; }

        /// <summary>Category name (e.g., "Food", "Rent", "Salary").</summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>Transactions using this category.</summary>
        public IReadOnlyCollection<FinancialTransaction> Transactions => _transactions.AsReadOnly();

        // ─── Factory ────────────────────────────────────────────────────────────

        /// <summary>Creates a new financial category for a user.</summary>
        public static FinancialCategory Create(Guid userId, string name)
        {
            ValidateName(name);
            return new FinancialCategory(Guid.NewGuid(), userId, name);
        }

        // ─── Domain Methods ─────────────────────────────────────────────────────

        /// <summary>Renames the category.</summary>
        public void Rename(string newName)
        {
            ValidateName(newName);
            Name = newName;
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
