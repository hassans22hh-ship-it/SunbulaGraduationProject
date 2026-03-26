using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.ValueOpjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UserIdentityInfrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly UserIdentityDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(UserIdentityDbContext context)
        {
            _context = context;
            _dbSet = context.Set<User>();
        }

        // ═══════════════════════════════════════════════════════════
        // BASIC CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        /// <summary>
        /// Adds multiple users to the database.
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Updates an existing user.
        /// Note: Changes are tracked by EF Core, actual save happens in UnitOfWork.SaveChangesAsync.
        /// </summary>
        public void Update(User entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Soft deletes a user by marking IsDeleted = true.
        /// </summary>
        public void Delete(User entity)
        {
            entity.MarkAsDeleted(); // Soft delete
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Soft deletes multiple users.
        /// </summary>
        public void DeleteRange(IEnumerable<User> entities)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }
            _dbSet.UpdateRange(entities);
        }

        // ═══════════════════════════════════════════════════════════
        // QUERY OPERATIONS - BY ID
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Gets a user by ID (without related entities).
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted) // Soft delete filter
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Gets a user by ID with all refresh tokens loaded.
        /// </summary>
        public async Task<User?> GetByIdWithRefreshTokensAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.RefreshTokens) // Eager loading
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByIdWithSettingsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Settings)
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════
        // QUERY OPERATIONS - BY EMAIL
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Gets a user by email address.
        /// </summary>
        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            var emailValue = email.Value;
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => EF.Property<string>(u, "_email") == emailValue, cancellationToken);
        }

        /// <summary>
        /// Checks if an email already exists in the database.
        /// Used during registration to prevent duplicate emails.
        /// </summary>
        public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
        {
            var emailValue = email.Value;
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .AnyAsync(u => EF.Property<string>(u, "_email") == emailValue, cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════
        // QUERY OPERATIONS - BY REFRESH TOKEN
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Gets a user by their refresh token.
        /// Used during token refresh flow.
        /// </summary>
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.RefreshTokens) // Need tokens to validate
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(
                    u => u.RefreshTokens.Any(rt => rt.Token == refreshToken),
                    cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════
        // GENERIC QUERY OPERATIONS - WITH EXPRESSIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Gets all users (with soft delete filter).
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Finds users matching a predicate.
        /// Example: FindAsync(u => u.FirstName.Contains("Ahmed"))
        /// </summary>
        public async Task<IEnumerable<User>> FindAsync(
            Expression<Func<User, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the first user matching a predicate, or null.
        /// Example: FirstOrDefaultAsync(u => u.Email.Value == "test@test.com")
        /// </summary>
        public async Task<User?> FirstOrDefaultAsync(
            Expression<Func<User, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Checks if any user matches a predicate.
        /// Example: AnyAsync(u => u.CoinBalance > 1000)
        /// </summary>
        public async Task<bool> AnyAsync(
            Expression<Func<User, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Counts users matching a predicate (or all users if predicate is null).
        /// Example: CountAsync(u => u.IsActive)
        /// </summary>
        public async Task<int> CountAsync(
            Expression<Func<User, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => !u.IsDeleted);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync(cancellationToken);
        }
    }
}
