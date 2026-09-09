using AutoMapper;
using Library.Entity.Concrete.Operations;
using Library.Model.Dtos.Operations;

namespace Library.Business.Mappings;

public class LostBookProfile : Profile
{
    public LostBookProfile()
    {
        CreateMap<LostBook, LostBookListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src => $"{src.Member.User.FirstName} {src.Member.User.LastName}"))
            .ForMember(dest => dest.MemberNumber, opt => opt.MapFrom(src => src.Member.MemberNumber))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookCopy.Book.Title))
            .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => src.BookCopy.Book.ISBN));

        //aynı isim diğerleri ama sonra kontrol edeyim bazen gelmiyor.
    }
}