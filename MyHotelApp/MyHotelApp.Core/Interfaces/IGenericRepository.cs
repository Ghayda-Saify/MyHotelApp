using MyHotelApp.Domain.Entities;
using System.Linq.Expressions;

namespace MyHotelApp.Domain.Interfaces;
/// <summary>
/// This is an interface to generate all CURD methods for Entities
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGenericRepository<T> where T:BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> filter);
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}