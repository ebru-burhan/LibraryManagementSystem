namespace Library.Model.Dtos.Membership;

public class MembershipApplicationDto
{
    public int Id { get; set; }

    // İsmini daha açıklayıcı ve net yaptık
    public string ApplicationStatus { get; set; } = null!;
}