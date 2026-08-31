using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;

namespace Library.Entity.Concrete.Auth;

public class User : AuditableEntity
{
    // Hesap Bilgileri
    public string Email { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;


    public string? PasswordResetCode { get; set; }
    public DateTime? PasswordResetCodeExpiration { get; set; }


    // Çekirdek Kişisel ve İletişim Bilgileri
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? IdentityNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;


    // Onaylar
    public bool IsKvkkApproved { get; init; }
    public bool IsTermsAccepted { get; set; }

    // İlişkiler
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public Member? Member { get; set; } // kütüphane üyesiyse dolu olur burası
}