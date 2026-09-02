namespace Library.Model.Dtos.Members;

public class MemberDetailDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? PictureUrl { get; set; }
    public string Status { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public string? MembershipType { get; set; }
    public DateTime RegistrationDate { get; set; }
    public decimal UnpaidDebtAmount { get; set; }

    public List<MemberLoanDto> ActiveLoans { get; set; } = new();
    public List<MemberReservationDto> Reservations { get; set; } = new();
    public List<MemberPenaltyDto> Penalties { get; set; } = new();
}
