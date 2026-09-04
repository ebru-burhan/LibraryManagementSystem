using Library.Model.Dtos.Members;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IMemberService
{
    Task<IDataResult<MemberDirectoryDto>> GetAllAsync(string? statusCode, string? search);
    Task<IDataResult<MemberDetailDto>> GetByIdAsync(Guid id); 
    Task<IResult> UpdateStatusAsync(Guid id, string statusCode); 
    Task<IResult> DeleteAsync(Guid id); 
}