using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Lookups;

namespace Library.Entity.Concrete.Membership;

public class MembershipApplication : AuditableEntity
{
    //bu başvuru bir kere girilir güncellenmeyen propertler barındırıyor init ile sağladık  ayrıca güncelenen status de. sadece onu update edilir bırakıcaz
    //audtable de olan update time başvuru oluşurken boş ama statü değiştiğinde update time state in değişim saati olacak


    // Kişisel Bilgiler
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string IdentityNumber { get; init; } = null!;
    public DateOnly DateOfBirth { get; init; }

    // İletişim Bilgileri
    public string PhoneNumber { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Address { get; init; }

    // Hesap Bilgileri
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Onaylar
    public bool IsKvkkApproved { get; init; }
    public bool IsTermsAccepted { get; set; }

    // Başvuru Durumları İlişkisi (Onay Bekliyor, Onaylandı, Reddedildi vb.)
    public int ApplicationStatusId { get; set; }
    public MembershipApplicationStatus ApplicationStatus { get; set; } = null!;
}