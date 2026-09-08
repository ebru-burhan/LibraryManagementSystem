using AutoMapper;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Model.Dtos.Membership;

namespace Library.Business.Mappings;

public class MembershipProfile : Profile
{
    public MembershipProfile()
    {


        // Sadece bu satıra ExternalId eşleşmesini ekledik:
        CreateMap<MembershipType, MembershipTypeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId));

        CreateMap<MembershipApplication, MembershipApplicationDto>()
           // FirstName, LastName ve Email otomatik eşleşir.

           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
           .ForMember(dest => dest.ApplicationStatus, opt => opt.MapFrom(src => src.ApplicationStatus.Code))

            // Eğer entity içindeki property adları farklıysa explicit (açık) bağlayabiliriz:
           .ForMember(dest => dest.MembershipType, opt => opt.MapFrom(src => src.MembershipType));

    }
}