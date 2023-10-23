﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace RENHelpers.DataAccessHelpers.DatabaseHelpers;

/// <summary>
///     Represents a generic repository for database operations on a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type to work with.</typeparam>
public class RENRepository<TEntity> : IRENRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RENRepository{TEntity}" /> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RENRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException("context");
        _dbSet = context.Set<TEntity>();
    }

    #region Create

    /// <summary>
    ///     Inserts a single entity into the database.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    public virtual void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <summary>
    ///     Inserts a single entity into the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert asynchronously.</param>
    public virtual async Task InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    ///     Inserts a list of entities into the database.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    public virtual void Insert(List<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /// <summary>
    ///     Inserts a list of entities into the database asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to insert asynchronously.</param>
    public virtual async Task InsertAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    #region Update

    /// <summary>
    ///     Updates a single entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public virtual void Update(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    ///     Updates a single entity in the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update asynchronously.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    #endregion

    #region Read

    /// <summary>
    ///     Gets a queryable collection of entities from the database.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A queryable collection of entities.</returns>
    public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        IQueryable<TEntity> query = _dbSet;
        include?.Invoke(query);

        if (isReadOnly)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return orderBy != null ? orderBy(query) : query;
    }

    /// <summary>
    ///     Gets a queryable collection of entities from the database asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A task representing the queryable collection of entities.</returns>
    public virtual Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        IQueryable<TEntity> query = _dbSet;
        include?.Invoke(query);

        if (isReadOnly)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return Task.FromResult(orderBy != null ? orderBy(query) : query);
    }

    /// <summary>
    ///     Gets a list of entities from the database.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A list of entities.</returns>
    public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        return GetQueryable(filter, orderBy, include, isReadOnly).ToList();
    }

    /// <summary>
    ///     Gets a list of entities from the database asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A task representing the list of entities.</returns>
    public virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        return GetQueryableAsync(filter, orderBy, include, isReadOnly).Result.ToListAsync();
    }

    /// <summary>
    ///     Gets a single entity from the database.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A single entity or null if not found.</returns>
    public virtual TEntity? GetSingle(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false)
    {
        return GetQueryable(filter, orderBy, include, isReadOnly).SingleOrDefault();
    }

    /// <summary>
    ///     Gets a single entity from the database asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A task representing the single entity or null if not found.</returns>
    public virtual Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false)
    {
        return GetQueryableAsync(filter, orderBy, include, isReadOnly).Result.SingleOrDefaultAsync();
    }

    #endregion

    #region Delete

    /// <summary>
    ///     Deletes a single entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    ///     Deletes a single entity from the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete asynchronously.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Deletes a list of entities from the database.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    public virtual void Delete(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    ///     Deletes a list of entities from the database asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DeleteAsync(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    #endregion
}