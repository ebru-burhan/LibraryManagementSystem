namespace Library.Model.Dtos.Membership;

public class CreateMembershipApplicationDto
{

    public string? PictureUrl { get; set; }
    public string? DocumentUrl { get; set; }

    // Kişisel Bilgiler
    public string IdentityNumber { get; set; } = null!;

    // Frontend'den tarih formatı string veya datetime gelebilir, DateOnly'e Business katmanında çevirmek daha güvenlidir.
    public DateTime DateOfBirth { get; set; }

    // İletişim Bilgileri
    public string PhoneNumber { get; set; } = null!;
 
    public string Address { get; set; } = null!;

    // Frontend kod gönderir (STUDENT); ID'yi Business lookup'tan çözer.
    public string MembershipTypeCode { get; set; } = null!;
}