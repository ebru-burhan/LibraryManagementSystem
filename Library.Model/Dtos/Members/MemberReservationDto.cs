namespace Library.Model.Dtos.Members;

public class MemberReservationDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = null!;
    public string? Authors { get; set; }
    public int QueueNumber { get; set; }
    public DateTime ReservationDate { get; set; }
    public string Status { get; set; } = null!;
}
