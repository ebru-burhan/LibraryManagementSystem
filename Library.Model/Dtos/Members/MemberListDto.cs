namespace Library.Model.Dtos.Members;

public class MemberListDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? PictureUrl { get; set; }
    public string Status { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public decimal UnpaidDebtAmount { get; set; }
}
