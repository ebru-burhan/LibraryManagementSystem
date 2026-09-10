namespace Library.Model.Dtos.Operations;

public class CreateReservationDto
{
    public Guid BookExternalId { get; set; }
}

public class ReservationListDto : BaseExternalDto
{
    public string BookTitle { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public DateTime ReservationDate { get; set; }
    public int QueueNumber { get; set; } 
    public string Status { get; set; } = null!; // Bekliyor Hazır 
}