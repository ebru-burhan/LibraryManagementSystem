namespace Library.Model.Dtos.Members;

public class MemberDetailDto : MemberListDto
{
 

    public List<MemberLoanDto> ActiveLoans { get; set; } = new();
    public List<MemberReservationDto> Reservations { get; set; } = new();
    public List<MemberPenaltyDto> Penalties { get; set; } = new();
}
