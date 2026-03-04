

namespace TaskDomain.Exceptions
{
    public sealed class CategoryNotFoundException:Exception
    {
        public CategoryNotFoundException(Guid categoryId)
        : base($"Category with ID '{categoryId}' was not found")
        {
            CategoryId = categoryId;
        }

        public Guid CategoryId { get; }
    }
}
