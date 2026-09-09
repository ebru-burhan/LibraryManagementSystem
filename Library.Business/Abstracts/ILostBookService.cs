using Library.Model.Dtos.Operations;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface ILostBookService
{
    Task<IResult> ReportLostBookAsync(ReportLostBookDto dto);
    Task<IDataResult<LostBookKpiDto>> GetLostBookKpisAsync();
    Task<IDataResult<List<LostBookListDto>>> GetAllLostBooksAsync();
}