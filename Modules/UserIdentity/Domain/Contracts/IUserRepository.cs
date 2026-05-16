using Domain.Entities;
using Domain.Entities.ValueObjects;
using SharedKernel;

namespace Domain.Contracts
{
    public interface IUserRepository:IRepository<User>
    {

        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithRefreshTokensAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithSettingsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> TryDeductCoinsAtomicAsync(Guid userId, int amount, CancellationToken cancellationToken = default);
    }
}
