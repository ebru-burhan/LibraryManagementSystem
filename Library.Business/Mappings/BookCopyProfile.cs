using AutoMapper;
using Library.Entity.Concrete.Catalog;
using Library.Model.Dtos.Catalog;

namespace Library.Business.Mappings;

public class BookCopyProfile : Profile
{
    public BookCopyProfile()
    {
        // 1. Create İşlemi: DTO -> Entity
        CreateMap<CreateBookCopyDto, BookCopy>()
            .ForMember(dest => dest.BookId, opt => opt.Ignore());

        // 2. Listeleme: Entity -> BookCopyListDto
        CreateMap<BookCopy, BookCopyListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.BookAuthorsNameList, opt => opt.MapFrom(src =>
                src.Book != null && src.Book.BookAuthors != null
                    ? src.Book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").ToList()
                    : new List<string>()));

        // 3. Detay: Entity -> BookCopyDetailDto (ListDto'dan türediği için üst alanları alır, ekstra ShelfLocation ve BookAuthors'u mapleriz)
        CreateMap<BookCopy, BookCopyDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.BookAuthorsNameList, opt => opt.MapFrom(src =>
                src.Book != null && src.Book.BookAuthors != null
                    ? src.Book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").ToList()
                    : new List<string>()))
            .ForMember(dest => dest.ShelfLocation, opt => opt.MapFrom(src => src.ShelfLocation))
            .ForMember(dest => dest.BookAuthors, opt => opt.MapFrom(src =>
                src.Book != null && src.Book.BookAuthors != null
                    ? src.Book.BookAuthors.Select(ba => new BookAuthorDto
                    {
                        Id = ba.Author.ExternalId,
                        FullName = $"{ba.Author.FirstName} {ba.Author.LastName}"
                    }).ToList()
                    : new List<BookAuthorDto>()));
    }
}