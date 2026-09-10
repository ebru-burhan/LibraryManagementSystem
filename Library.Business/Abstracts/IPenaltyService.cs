using Library.Model.Results;
using Library.Model.Dtos.Operations; // Birazdan bu DTO'yu oluşturacağız

namespace Library.Business.Abstracts;

public interface IPenaltyService
{
    // Üyenin tüm cezalarını getirme
    Task<IDataResult<List<PenaltyListDto>>> GetPenaltiesByMemberIdAsync(Guid memberExternalId);

    // Ceza ödeme işlemi
    Task<IResult> PayPenaltyAsync(Guid penaltyExternalId);

    Task<IDataResult<List<PenaltyListDto>>> GetPenaltiesByUserIdAsync(int userId);
}