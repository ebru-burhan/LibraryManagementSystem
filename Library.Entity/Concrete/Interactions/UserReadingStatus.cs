using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;
using Library.Entity.Constants;
//book için using

namespace Library.Entity.Concrete.Interactions;

public class UserReadingStatus : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // TODO: book gelecek

    // Varsayılan olarak başlangıç durumu
    public string Status { get; set; } = Statuses.Reading.CurrentlyReading;

    // Sadece tamamlandığında dolar
    public DateTime? CompletedAt { get; set; }

}