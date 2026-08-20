using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Catalog;

namespace Library.Entity.Concrete.Operations;

// Kayıp Kitap Yönetimi 
public class LostBook : AuditableEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;

    public DateTime DeclaredDate { get; set; } = DateTime.UtcNow; // Bildirim tarihi
    public decimal BookValue { get; set; } // Kitap bedeli
    public bool IsResolved { get; set; } = false; // Tahsilat durumu veya bulundu mu
}