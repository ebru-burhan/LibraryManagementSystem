namespace Library.Model.Dtos.Members;

public class MemberPenaltyDto
{
    public int Id { get; set; }
    public string Reason { get; set; } = null!;
    public string? RelatedBookTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DelayDays { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
}
