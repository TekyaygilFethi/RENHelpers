using EFCore.BulkExtensions;
using System.Linq.Expressions;

namespace RENHelpers.DataAccessHelpers;

/// <summary>
///     Represents a generic repository interface for performing CRUD operations on a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type to work with.</typeparam>
public interface IRENRepository<TEntity> where TEntity : class
{
    #region Create

    /// <summary>
    ///     Inserts a single entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    void Insert(TEntity entity);

    /// <summary>
    ///     Inserts a single entity into the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Inserts list of entities into the repository.
    /// </summary>
    /// <param name="entities">The entity list to insert.</param>
    void Insert(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Inserts list of entities into the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entity list to insert asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Bulk inserts a list of entities into the repository.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    void BulkInsert(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null);

    /// <summary>
    ///     Bulk inserts a list of entities into the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to insert asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BulkInsertAsync(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null, CancellationToken cancellationToken = default);
    #endregion

    #region Read

    /// <summary>
    ///     Gets a queryable collection of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a queryable collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the queryable collection of entities.</returns>
    Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A list of entities.</returns>
    List<TEntity> GetList(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the list of entities.</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a single entity from the repository.
    /// </summary>
    /// <param name="filter">Required filter expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A single entity or null if not found.</returns>
    TEntity? GetSingle(Expression<Func<TEntity, bool>> filter,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null,
        bool isReadOnly = false);

    /// <summary>
    ///     Gets a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="filter">Required filter expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the single entity or null if not found.</returns>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter,
        Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>>? include = null,
        bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    ///     Updates a single entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    ///     Updates a single entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Bulk updates multiple entities in the repository.
    /// </summary>
    /// <param name="entity">The entities to update.</param>
    void BulkUpdate(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null);

    /// <summary>
    ///     Bulk updates multiple entities in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entities to update asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BulkUpdateAsync(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null, CancellationToken cancellationToken = default);
    #endregion

    #region Delete

    /// <summary>
    ///     Deletes a single entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    ///     Deletes a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a list of entities from the database.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    void Delete(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Deletes a list of entities from the database asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Bulk deletes a list of entities from the repository.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>{{}
    void BulkDelete(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null);

    /// <summary>
    ///     Bulk deletes a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <param name="cancellationToken">Optional cancellationToken.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BulkDeleteAsync(IEnumerable<TEntity> entities, BulkConfig? bulkConfig = null, CancellationToken cancellationToken = default);
    #endregion
}