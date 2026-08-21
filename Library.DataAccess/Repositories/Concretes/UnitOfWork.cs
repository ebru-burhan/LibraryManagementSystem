using Library.DataAccess.Contexts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Abstract;

namespace Library.DataAccess.Repositories.Concretes;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _disposed = false;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // İstendiğinde ilgili entity için GenericRepository üretir
    //Bu metot (Repository Factory yani Repository Fabrikası olarak geçer),
    //projede her tablo için ayrı ayrı repository sınıfı yazma derdinden bizi kurtaran akıllı bir kapıdır.   ben eski projelerimde tek tek yazmıştım :((
    public IGenericRepository<T> GetRepository<T>() where T : class, IEntity
    {
        return new GenericRepository<T>(_context);
    }

    // Tüm değişiklikleri veritabanına kaydeder
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Kaynakları güvenli bir şekilde serbest bırakır (Memory leak önleyici
    //o yüzden Iunitofwork e IAsyncDisposable implment edildi. bi nedeni de bu yani
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                await _context.DisposeAsync();
            }
            _disposed = true;
        }
    }
}