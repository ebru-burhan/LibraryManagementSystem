using Library.Model.Dtos.Roles;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IBookCopyService
{
    Task<IResult> CreateBookCopyAsync(CreateBookCopyDto bookCopyDto);
}