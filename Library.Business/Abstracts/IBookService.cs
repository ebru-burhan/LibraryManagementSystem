using Library.Model.Dtos.Catalog;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IBookService
{
    Task<IDataResult<List<BookListDto>>> GetAllAsync();
    Task<IDataResult<BookDetailDto>> GetByIdAsync(Guid id);
    Task<IResult> AddAsync(CreateBookDto dto);
    Task<IResult> DeleteAsync(Guid id);
}