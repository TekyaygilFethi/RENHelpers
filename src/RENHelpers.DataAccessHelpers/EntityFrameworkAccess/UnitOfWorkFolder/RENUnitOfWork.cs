using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace RENHelpers.DataAccessHelpers.DatabaseHelpers;

/// <summary>
///     Represents a unit of work for managing database transactions and repositories in a generic context.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class RENUnitOfWork<TDbContext> : IRENUnitOfWork<TDbContext> where TDbContext : DbContext
{
    protected readonly TDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public bool IsInTransaction => _currentTransaction != null;

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
    /// <returns>True if the changes are successfully saved; otherwise, false.</returns>
    public virtual void SaveChanges(bool createInnerTransaction = false)
    {
        if (!createInnerTransaction)
        {
            _context.SaveChanges();
            return;
        }

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
    /// <returns>A task representing the success of saving the changes (true if successful; otherwise, false).</returns>
    public virtual async Task SaveChangesAsync(bool createInnerTransaction = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!createInnerTransaction)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

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

    /// <summary>
    ///    Creates a transaction.
    /// </summary>
    /// <returns>An instance of the created transaction object.</returns>
    public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = _context.Database.BeginTransaction(isolationLevel);
    }

    /// <summary>
    ///    Creates a transaction asynchronously.
    /// </summary>
    /// <returns>An instance of the created transaction object.</returns>
    public virtual async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    /// <summary>
    ///    Commits the changes on transaction object.
    /// </summary>
    public void CommitTransaction()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("There is no transaction to commit.");

        _currentTransaction.Commit();
        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    /// <summary>
    ///    Commits the changes on transaction object asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("There is no transaction to commit.");

        await _currentTransaction.CommitAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    /// <summary>
    ///    Rollbacks the changes on transaction object.
    /// </summary>
    public void RollbackTransaction()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("There is no transaction to commit.");

        _currentTransaction.Commit();
        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    /// <summary>
    ///    Rollbacks the changes on transaction object asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("There is no transaction to rollback.");

        await _currentTransaction.RollbackAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    /// <summary>
    ///    Disposes the Unit Of Work object.
    /// </summary>
    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}