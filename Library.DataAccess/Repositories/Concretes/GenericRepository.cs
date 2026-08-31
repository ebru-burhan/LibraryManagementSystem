using Library.DataAccess.Contexts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Library.DataAccess.Repositories.Concretes;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    //book repo falan gerekirse protected
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    // Dependency Injection: Entity Framework Context'ini dışarıdan alıyoruz
    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>(); // Hangi entity (Book, Member vb.) geldiyse onun tablosuna bağlan
    }

    public async Task<T?> GetByIdAsync(int id, bool tracking = true)
    {
        // Eğer tracking false ise AsNoTracking çalıştır (Performans modu)
        if (!tracking)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (!tracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool tracking = true)
    {
        var query = _dbSet.Where(predicate);
        if (!tracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }



    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Eğer dışarıdan include parametreleri gönderilmişse, bunları sorguya ekle (JOIN işlemi)
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.Where(expression).ToListAsync();
    }



    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        // Eğer nesne sadece oluşturulabilir bir log/audit kaydiyse güncellenemez!
        if (entity is CreationAuditedEntity)
        {
            throw new InvalidOperationException("Log ve denetim kayıtları (CreationAuditedEntity) güncellenemez!");
        }
        _dbSet.Update(entity); // Dışarıdan gelen her nesneyi direkt update etmeye çalışır riskk burda!!! 
        //service de getbyıd ile çek öyle güncelle kaydet. // TODO: başka yol bak var mı
    }

    public void Delete(T entity)
    {

        if (entity is CreationAuditedEntity)
        {
            throw new InvalidOperationException("Log ve denetim kayıtları (CreationAuditedEntity) silinemez!");
        }

        if (entity is AuditableEntity auditableEntity)
        {
            // Soft Delete: Veriyi silmiyoruz, durumunu güncelliyoruz!
            auditableEntity.IsDeleted = true;
            auditableEntity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        else
        {
            // Hard Delete: Sadece BaseEntity veya ara tablolarsa izin ver
            _dbSet.Remove(entity);
        }
    }
}