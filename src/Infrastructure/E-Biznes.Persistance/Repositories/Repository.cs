using AzBinaTeam.Application.Abstracts.Repositories;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AzBinaTeam.Persistance.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
{
    private readonly AppDbContext _context;

    private readonly DbSet<T> Table;

    public Repository(AppDbContext context)
    {
        _context = context;
        Table = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await Table.AddAsync(entity);
    }

    public void Update(T entity)
    {
        Table.Update(entity);
    }

    public void Delete(T entity)
    {
        Table.Remove(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Table.FindAsync(id);
    }

    public IQueryable<T> GetByFiltered(Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[]? include = null,
        bool isTracking = false)
    {
        IQueryable<T> query = Table;
        if (predicate is not null)
            query = query.Where(predicate);

        if (include is not null)
        {
            foreach (var includeExpression in include)
                query = query.Include(includeExpression);
        }

        if (!isTracking)
            return query.AsNoTracking();

        return query;

    }

    public IQueryable<T> GetAll(bool isTracking = false)
    {
        if (!isTracking)
            return Table.AsNoTracking();
        return Table;
    }


    public IQueryable<T> GetAllFiltered(Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[]? include = null,
        Expression<Func<T, object>>? orderby = null,
        bool isOrderByAsc = true,
        bool isTracking = false)
    {
        IQueryable<T> query = Table;
        if (predicate is not null)
            query = query.Where(predicate);

        if (include is not null)
        {
            foreach (var includeExpression in include)
                query = query.Include(includeExpression);
        }

        if (orderby is not null)
        {
            if (isOrderByAsc)
                query = query.OrderBy(orderby);
            else
                query = query.OrderByDescending(orderby);
        }

        if (!isTracking)
            return query.AsNoTracking();

        return query;
    }

    public async Task SaveChangeAsync()
    {
        await _context.SaveChangesAsync();
    }
}
