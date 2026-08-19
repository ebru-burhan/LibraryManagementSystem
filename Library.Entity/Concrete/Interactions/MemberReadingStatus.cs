using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
using Library.Entity.Constants;

namespace Library.Entity.Concrete.Interactions;


// TODO: SONRA to read mantığı için aslında creationAud classından inherite edebiliriz
public class MemberReadingStatus : BaseEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    // Varsayılan olarak başlangıç durumu
    public string Status { get; set; } = Statuses.Reading.CurrentlyReading;

    //okumaya başlama saati de koysam mı ne kadar sürede okudu falan hatta bassın süre tutsun durdursun
    //tekrar okuyacağında bassın tekrar devam etsin okuma hızı gelişimini bile hesaplarız görürüz // TODO:SONRA stajdan sonra başka projede devam etmimari gelişimine
    // Butona ilk basıldığında DateTime.UtcNow atanır başlama ve bitiş süresi görürüz. bi nebze member hakkında bilgi verir.
    public DateTime? StartedAt { get; set; }
    // Sadece tamamlandığında dolar okudum dediğinde butona basınca o saat gelmeli
    public DateTime? CompletedAt { get; set; }

}