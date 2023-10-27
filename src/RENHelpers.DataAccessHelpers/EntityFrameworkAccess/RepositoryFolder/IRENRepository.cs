using System.Linq.Expressions;

namespace RENHelpers.DataAccessHelpers.DatabaseHelpers;

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
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Inserts a list of entities into the repository.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    void Insert(List<TEntity> entities);

    /// <summary>
    ///     Inserts a list of entities into the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to insert asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

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
    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a queryable collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the queryable collection of entities.</returns>
    Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A list of entities.</returns>
    List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the list of entities.</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a single entity from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A single entity or null if not found.</returns>
    TEntity? GetSingle(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false);

    /// <summary>
    ///     Gets a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the single entity or null if not found.</returns>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
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
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a list of entities from the repository.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    void Delete(List<TEntity> entities);

    /// <summary>
    ///     Deletes a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion
}