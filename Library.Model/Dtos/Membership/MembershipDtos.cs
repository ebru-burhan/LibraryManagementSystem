namespace Library.Model.Dtos.Membership;

// 1. Yeni Başvuru Oluşturma İstek Dto'su
public class CreateMembershipApplicationDto
{
    public string? PictureUrl { get; set; }
    public string? DocumentUrl { get; set; }

    // Kişisel Bilgiler
    public string IdentityNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    // İletişim Bilgileri
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;

    // Frontend kod gönderir (STUDENT) dto olunca formdata sıkıntı çıkarıyor .
    public string MembershipTypeCode { get; set; } = null!;
}

// 2. Başvuru Listeleme ve Detay Dto'su
public class MembershipApplicationDto : BaseExternalDto
{
    public string? PictureUrl { get; set; }
    public string? DocumentUrl { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string IdentityNumber { get; init; } = null!;
    public DateOnly DateOfBirth { get; init; }

    public DateTime CreatedAt { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;

    public string ApplicationStatus { get; set; } = null!;
    public MembershipTypeDto MembershipType { get; set; } = null!;
}

// 3. Üyelik Türü Dto'su (Dropdown ve detaylar için)
public class MembershipTypeDto : BaseExternalDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}