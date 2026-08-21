using Library.Entity.Abstract;

namespace Library.DataAccess.Repositories.Abstracts;

public interface IUnitOfWork : IAsyncDisposable
{
    // İstediğimiz entity için generic repository'yi hızlıca çekmemizi sağlar
    IGenericRepository<T> GetRepository<T>() where T : class, IEntity;

    // Tüm değişiklikleri tek bir transaction altında veritabanına yazar
    //tek seferde veritabanına kaydeder. veri bütünlüğü bozulmaz
    //çünkü bi yerde loan kaydı oluşturuldu o kitap raftan ödünçe geçicek ikisi ayrı zmanda olursa veri tutarsızlığı
    Task<int> CompleteAsync();
}