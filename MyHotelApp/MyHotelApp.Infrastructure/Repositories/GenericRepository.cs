using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using System.Linq.Expressions;

namespace MyHotelApp.Infrastructure.Repositories;
/// <summary>
/// This is a generic Class to handle all the CURD methods for all Entities
/// </summary>
/// <param name="context"></param>
/// <typeparam name="T"></typeparam>
public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T>
    where T : BaseEntity
{
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await context.Set<T>()
            .Where(filter)
            .ToListAsync();
    }
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}