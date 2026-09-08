using Library.Model.Dtos.Members;

namespace Library.Model.Dtos.Penalties;

public class PenaltyListDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public string PenaltyType { get; set; } = null!;
    public string? BookTitle { get; set; }

    // dto gerek yok burda ya sadece ismi gerek id ile de tıklayabilir bence
    public Guid MemberExternalId { get; set; }
    public string MemberFullName { get; set; } = null!;
}