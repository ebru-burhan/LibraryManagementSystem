using Library.Model.Dtos.Members;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IMemberService
{
    Task<IDataResult<MemberDirectoryDto>> GetAllAsync(string? statusCode, string? search);
    Task<IDataResult<MemberDetailDto>> GetByIdAsync(int id);
    Task<IResult> UpdateStatusAsync(int id, string statusCode);
    Task<IResult> DeleteAsync(int id);
}
