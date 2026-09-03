using AutoMapper;
using Library.Entity.Concrete.Catalog;
using Library.Model.Dtos.Catalog;




public class BookProfile : Profile
{

    public BookProfile()
    {
        CreateMap<Book, BookListDto>()
    .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
        src.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").ToList()
    ));
    }

}










