using System.Linq.Expressions;
using Library.Entity.Abstract;

namespace Library.DataAccess.Repositories.Abstracts;

public interface IGenericRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id, bool tracking = true);
    Task<IEnumerable<T>> GetAllAsync(bool tracking = true);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool tracking = true);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

    IQueryable<T> Query(bool tracking = false);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}