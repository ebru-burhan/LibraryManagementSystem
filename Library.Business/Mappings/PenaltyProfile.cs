using AutoMapper;
using Library.Entity.Concrete.Operations;
using Library.Model.Dtos.Penalties;

namespace Library.Business.Mappings;

public class PenaltyProfile : Profile
{
    public PenaltyProfile()
    {
        CreateMap<Penalty, PenaltyListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.PenaltyType, opt => opt.MapFrom(src => src.PenaltyType.Name))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src =>
                src.Loan != null ? src.Loan.BookCopy.Book.Title : "Genel Ceza"))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Loan!.BookCopy!.Barcode))

            .ForMember(dest => dest.MemberExternalId, opt => opt.MapFrom(src => src.Member.ExternalId))
            .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src =>
                $"{src.Member.User.FirstName} {src.Member.User.LastName}"));
    }
}