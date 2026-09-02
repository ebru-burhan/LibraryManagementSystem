using AutoMapper;
using Library.Entity.Concrete.Membership;
using Library.Model.Dtos.Membership;

namespace Library.Business.Mappings;

public class MembershipProfile : Profile
{
    public MembershipProfile()
    {
        CreateMap<MembershipApplication, MembershipApplicationDto>()
           // FirstName, LastName ve Email otomatik eşleşir.
           // Sadece Lookup tablosundan gelen statü ismini (veya kodunu) manuel belirtiyoruz:
           .ForMember(dest => dest.ApplicationStatus, opt => opt.MapFrom(src => src.ApplicationStatus.Code))
           .ForMember(dest => dest.MembershipType, opt => opt.MapFrom(src => src.MembershipType.Code))
           .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))
           .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.DocumentUrl));

        ;


    }
}