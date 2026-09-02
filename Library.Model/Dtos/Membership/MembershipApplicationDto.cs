namespace Library.Model.Dtos.Membership;

public class MembershipApplicationDto
{

    public int Id { get; set; }
    public string? PictureUrl { get; set; }
    public string? DocumentUrl { get; set; }

    // Doğrudan entity'deki property isimleriyle aynı
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string IdentityNumber { get; init; } = null!;
    public DateOnly DateOfBirth { get; init; }

    public DateTime CreatedAt { get; set; } // Eklenen alan
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;

    // İsmini daha açıklayıcı ve net yaptık
    public string ApplicationStatus { get; set; } = null!;
    public string MembershipType { get; set; } = null!;
}