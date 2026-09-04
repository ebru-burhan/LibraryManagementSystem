using AutoMapper;
using Library.Entity.Concrete.Catalog;
using Library.Model.Dtos.Catalog;

namespace Library.Business.Mappings;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId));

        CreateMap<Book, BookDetailDto>()
            .IncludeBase<Book, BookListDto>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
                src.BookAuthors.Select(ba => new BookAuthorDto
                {
                    Id = ba.Author.ExternalId,
                    FullName = $"{ba.Author.FirstName} {ba.Author.LastName}".Trim()
                }).ToList()));

        // Yeni kayıt için Guid biz üretiyoruz!
        // TODO: DTO'dan gelen AuthorIds (Guid) listesini BookAuthor ara tablosuna çevirme işini Manager içinde yapacağız.
        CreateMap<CreateBookDto, Book>()
                    .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => Guid.NewGuid()));
    }

}







