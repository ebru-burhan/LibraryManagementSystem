using AutoMapper;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Model.Dtos.Membership;

namespace Library.Business.Mappings;

public class MembershipProfile : Profile
{
    public MembershipProfile()
    {


        CreateMap<MembershipType, MembershipTypeDto>();

        CreateMap<MembershipApplication, MembershipApplicationDto>()
           // FirstName, LastName ve Email otomatik eşleşir.

           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
           .ForMember(dest => dest.ApplicationStatus, opt => opt.MapFrom(src => src.ApplicationStatus.Code))
           .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))
           .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.DocumentUrl));



    }
}