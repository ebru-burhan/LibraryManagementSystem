using Library.Model.Dtos.Catalog;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IBookCopyService
{
    Task<IResult> CreateBookCopyAsync(CreateBookCopyDto bookCopyDto);

    Task<IDataResult<List<BookCopyListDto>>> GetAllBookCopiesAsync();
    Task<IDataResult<BookCopyDetailDto>> GetBookCopyByExternalIdAsync(Guid externalId);
}