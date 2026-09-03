using Library.Model.Dtos.Catalog;
using Library.Model.Dtos.Roles;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IBookService
{
    Task<IDataResult<List<BookListDto>>> GetAllBooksAsync();
}