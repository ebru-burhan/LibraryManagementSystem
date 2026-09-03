using AutoMapper;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Operations;
using Library.Entity.Constants;
using Library.Model.Dtos.Members;

namespace Library.Business.Mappings;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        // 1. MemberListDto Temel Eşleşmesi
        CreateMap<Member, MemberListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address ?? src.MembershipApplication.Address))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber ?? src.MembershipApplication.PhoneNumber))
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.MembershipApplication.PictureUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.UnpaidDebtAmount, opt => opt.MapFrom(src => src.Penalties.Where(p => !p.IsPaid).Sum(p => p.Amount)));

        // 2. MemberDetailDto Alt Koleksiyonları (Nested Mapping)
        CreateMap<Loan, MemberLoanDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookCopy.Book.Title))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => FormatAuthors(src.BookCopy.Book)))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate.Date < DateTime.UtcNow.Date || src.Status.Code == Statuses.Loan.Overdue || src.Status.Code == Statuses.Loan.Critical))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (src.DueDate.Date < DateTime.UtcNow.Date || src.Status.Code == Statuses.Loan.Overdue || src.Status.Code == Statuses.Loan.Critical) ? Statuses.Loan.Overdue : src.Status.Code));

        CreateMap<Reservation, MemberReservationDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => FormatAuthors(src.Book)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));

        CreateMap<Penalty, MemberPenaltyDto>()
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.PenaltyType.Name))
            .ForMember(dest => dest.RelatedBookTitle, opt => opt.MapFrom(src => src.Loan != null ? src.Loan.BookCopy.Book.Title : null))
            .ForMember(dest => dest.DelayDays, opt => opt.MapFrom(src => CalculateDelayDays(src)));

        // 3. MemberDetailDto Ana Eşleşmesi (MemberListDto'yu miras alır)
        CreateMap<Member, MemberDetailDto>()
            .IncludeBase<Member, MemberListDto>()
            // Listeleri filtreleyip sıralayarak eşleştir
            .ForMember(dest => dest.ActiveLoans, opt => opt.MapFrom(src => src.Loans.Where(l => l.ReturnDate == null).OrderByDescending(l => l.LoanDate)))
            .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations.Where(r => r.Status.Code == Statuses.Reservation.Waiting).OrderBy(r => r.QueueNumber)))
            .ForMember(dest => dest.Penalties, opt => opt.MapFrom(src => src.Penalties.OrderByDescending(p => p.CreatedAt)));
    }

    // Manager'dan taşınan özel formatlama metotları
    private static string? FormatAuthors(Entity.Concrete.Catalog.Book book)
    {
        if (book.BookAuthors == null || book.BookAuthors.Count == 0)
            return null;

        return string.Join(", ", book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}".Trim()));
    }

    private static int CalculateDelayDays(Penalty penalty)
    {
        if (penalty.Loan == null)
            return 0;

        var endDate = penalty.Loan.ReturnDate ?? DateTime.UtcNow;
        var days = (endDate.Date - penalty.Loan.DueDate.Date).Days;
        return days < 0 ? 0 : days;
    }
}