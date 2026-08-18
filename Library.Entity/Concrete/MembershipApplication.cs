using Library.Entity.Abstract;
using Library.Entity.Concrete.Lookups;

namespace Library.Entity.Concrete;

public class MembershipApplication : AuditableEntity
{
    // Kişisel Bilgiler
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    // İletişim Bilgileri
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }

    // Hesap Bilgileri
    public string UserName { get; set; } = null!;
    // Kullanıcı henüz onaylanmadığı için şifresini burada hash'li olarak bekletiyoruz
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    // Onaylar
    public bool IsKvkkApproved { get; set; }
    public bool IsTermsAccepted { get; set; }

    // Başvuru Durumları İlişkisi (Onay Bekliyor, Onaylandı, Reddedildi vb.)
    public int ApplicationStatusId { get; set; }
    public MembershipApplicationStatus ApplicationStatus { get; set; } = null!;
}