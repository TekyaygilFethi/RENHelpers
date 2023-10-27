using Microsoft.EntityFrameworkCore;

namespace RENHelpers.DataAccessHelpers.DatabaseHelpers;

/// <summary>
///     Represents a unit of work for managing database transactions and repositories in a generic context.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class RENUnitOfWork<TDbContext> : IRENUnitOfWork<TDbContext> where TDbContext : DbContext
{
    protected readonly TDbContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RENUnitOfWork{TDbContext}" /> class.
    /// </summary>
    /// <param name="context">The database context to be used within the unit of work.</param>
    public RENUnitOfWork(TDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(TDbContext));
    }

    /// <summary>
    ///     Commits the changes made in the unit of work to the database.
    /// </summary>
    public virtual void SaveChanges()
    {
        using var ctxTransaction = _context.Database.BeginTransaction();
        try
        {
            _context.SaveChanges();
            ctxTransaction.Commit();
        }
        catch (Exception)
        {
            ctxTransaction.Rollback();
            throw;
        }
    }

    /// <summary>
    ///     Commits the changes made in the unit of work to the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var ctxTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await ctxTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await ctxTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    ///     Gets a repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>An instance of the repository for the specified entity type.</returns>
    public virtual IRENRepository<TEntity>? GetRENRepository<TEntity>() where TEntity : class
    {
        return (IRENRepository<TEntity>?)Activator.CreateInstance(typeof(RENRepository<TEntity>), _context);
    }

    /// <summary>
    ///     Gets a repository for a specific entity type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>A task representing an instance of the repository for the specified entity type.</returns>
    public virtual Task<IRENRepository<TEntity>?> GetRENRepositoryAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return (IRENRepository<TEntity>?)Activator.CreateInstance(typeof(RENRepository<TEntity>), _context);
        }, cancellationToken);
    }
}