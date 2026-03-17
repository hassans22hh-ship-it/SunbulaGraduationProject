
namespace Application.TaskManagmentDTOS
{
    public sealed record CategoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Color { get; init; }
        public required DateTime CreatedAt { get; init; }
        public int TaskCount { get; init; }
    }
}
