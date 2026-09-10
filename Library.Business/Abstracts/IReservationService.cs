using Library.Model.Dtos.Operations;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IReservationService
{
    
    Task<IResult> CreateReservationAsync(int userId, CreateReservationDto dto);

    // Üyenin kendi profilinde "Rezervasyonlarım" tablosunu besler.
    Task<IDataResult<List<ReservationListDto>>> GetReservationsByMemberIdAsync(Guid memberExternalId);


    Task<IDataResult<List<ReservationListDto>>> GetReservationsByUserIdAsync(int userId);

    Task<IResult> CancelReservationAsync(int userId, Guid reservationExternalId);
    Task<IDataResult<List<ReservationListDto>>> GetAllReservationsAsync();
}