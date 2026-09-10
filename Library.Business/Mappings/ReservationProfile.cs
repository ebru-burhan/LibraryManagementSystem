using AutoMapper;
using Library.Entity.Concrete.Operations;
using Library.Model.Dtos.Operations;

namespace Library.Business.Mappings;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {

        CreateMap<Reservation, ReservationListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.Book.CoverImageUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));

        CreateMap<CreateReservationDto, Reservation>()
            // Bu alanları DTO'dan otomatik eşleyemeyiz, Manager içinde biz kendimiz (manuel) atayacağız!
            .ForMember(dest => dest.BookId, opt => opt.Ignore())    // Guidden inte çevir
            .ForMember(dest => dest.MemberId, opt => opt.Ignore())  // Tokendan gelen kimlikle buluruz
            .ForMember(dest => dest.StatusId, opt => opt.Ignore())  // Lookup tablosundan WAITING çek
            .ForMember(dest => dest.QueueNumber, opt => opt.Ignore()); // Sıra numarasını algoritma hesaplıcak
    }
}