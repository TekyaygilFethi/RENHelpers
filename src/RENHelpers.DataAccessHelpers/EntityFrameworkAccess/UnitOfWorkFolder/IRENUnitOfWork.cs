using Microsoft.EntityFrameworkCore;

namespace RENHelpers.DataAccessHelpers.DatabaseHelpers;

public interface IRENUnitOfWork<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    ///     Commits the changes made in the unit of work to the database.
    /// </summary>
    void SaveChanges();

    /// <summary>
    ///     Commits the changes made in the unit of work to the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>An instance of the repository for the specified entity type.</returns>
    IRENRepository<TEntity>? GetRENRepository<TEntity>() where TEntity : class;

    /// <summary>
    ///     Gets a repository for a specific entity type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>A task representing an instance of the repository for the specified entity type.</returns>
    Task<IRENRepository<TEntity>?> GetRENRepositoryAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;
}