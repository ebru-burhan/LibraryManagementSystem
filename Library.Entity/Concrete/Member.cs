using Library.Entity.Abstract;

namespace Library.Entity.Concrete;

public class Member : AuditableEntity
{
    // TODO: IsUnique() (Benzersiz) kuralı eklememiz yeterli olacak ==> fluent api configurasyonda yapalım entityler kalabalık olcak yoksa
    public string MemberNumber { get; set; } = null!; // Kütüphanenin atadığı özel numara (Örn: LUM-2023-001)

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // TODO: ekle ödünç, rezervasyon ve ceza ilişkileri 
}