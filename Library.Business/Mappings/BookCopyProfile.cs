using AutoMapper;
using Library.Entity.Concrete.Catalog;
namespace Library.Business.Mappings;

public class BookCopyProfile : Profile
{

    public BookCopyProfile()
    {
        CreateMap<BookCopy, CreateBookCopyDto>();
        CreateMap<CreateBookCopyDto, BookCopy>();
        //olmadı manuel yapcaz 
    }
    
}

