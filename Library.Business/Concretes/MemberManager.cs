using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Constants;
using Library.Model.Dtos.Members;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class MemberManager : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly IGenericRepository<MemberStatus> _statusRepository;

    public MemberManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _memberRepository = _unitOfWork.GetRepository<Member>();
        _statusRepository = _unitOfWork.GetRepository<MemberStatus>();
    }

    public async Task<IDataResult<MemberDirectoryDto>> GetAllAsync(string? statusCode, string? search)
    {
        var allStatusCodes = await _memberRepository.Query(tracking: false)
            .Include(m => m.Status)
            .Select(m => m.Status.Code)
            .ToListAsync();

        var query = BuildListQuery(tracking: false);

        if (!string.IsNullOrWhiteSpace(statusCode))
        {
            var normalized = statusCode.Trim().ToUpperInvariant();
            query = query.Where(m => m.Status.Code == normalized);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(m =>
                m.MemberNumber.ToLower().Contains(term) ||
                m.User.FirstName.ToLower().Contains(term) ||
                m.User.LastName.ToLower().Contains(term) ||
                m.User.Email.ToLower().Contains(term) ||
                (m.User.PhoneNumber != null && m.User.PhoneNumber.ToLower().Contains(term)));
        }

        var members = await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var items = members.Select(MapToListDto).ToList();

        var directory = new MemberDirectoryDto
        {
            Members = items,
            TotalCount = allStatusCodes.Count,
            ActiveCount = allStatusCodes.Count(x => x == Statuses.Member.Active),
            PassiveCount = allStatusCodes.Count(x => x == Statuses.Member.Passive),
            SuspendedCount = allStatusCodes.Count(x => x == Statuses.Member.Suspended)
        };

        return new SuccessDataResult<MemberDirectoryDto>(directory, "Üye listesi getirildi.");
    }

    public async Task<IDataResult<MemberDetailDto>> GetByIdAsync(int id)
    {
        var member = await BuildMemberQuery(tracking: false)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
            return new ErrorDataResult<MemberDetailDto>("Üye bulunamadı.");

        return new SuccessDataResult<MemberDetailDto>(MapToDetailDto(member), "Üye kartı getirildi.");
    }

    public async Task<IResult> UpdateStatusAsync(int id, string statusCode)
    {
        if (string.IsNullOrWhiteSpace(statusCode))
            return new ErrorResult("Üye durumu boş olamaz.");

        var member = await _memberRepository.GetByIdAsync(id, tracking: true);
        if (member == null)
            return new ErrorResult("Üye bulunamadı.");

        var statusId = await GetStatusIdByCodeAsync(statusCode.Trim().ToUpperInvariant());
        if (statusId == 0)
            return new ErrorResult("Geçersiz üye durumu.");

        if (member.StatusId == statusId)
            return new ErrorResult("Üye zaten bu durumda.");

        member.StatusId = statusId;
        _memberRepository.Update(member);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Üye durumu güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var member = await BuildMemberQuery(tracking: true)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
            return new ErrorResult("Üye bulunamadı.");

        var hasActiveLoan = member.Loans.Any(l => l.ReturnDate == null && !l.IsDeleted);
        if (hasActiveLoan)
            return new ErrorResult("Aktif ödüncü bulunan üye silinemez.");

        var hasUnpaidPenalty = member.Penalties.Any(p => !p.IsPaid && !p.IsDeleted);
        if (hasUnpaidPenalty)
            return new ErrorResult("Ödenmemiş cezası bulunan üye silinemez.");

        _memberRepository.Delete(member);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Üye kaydı silindi.");
    }

    private IQueryable<Member> BuildListQuery(bool tracking)
    {
        return _memberRepository.Query(tracking)
            .Include(m => m.User)
            .Include(m => m.Status)
            .Include(m => m.MembershipApplication)
            .Include(m => m.Penalties);
    }

    private IQueryable<Member> BuildMemberQuery(bool tracking)
    {
        return _memberRepository.Query(tracking)
            .Include(m => m.User)
            .Include(m => m.Status)
            .Include(m => m.MembershipApplication)
                .ThenInclude(a => a.MembershipType)
            .Include(m => m.Penalties)
                .ThenInclude(p => p.PenaltyType)
            .Include(m => m.Penalties)
                .ThenInclude(p => p.Loan!)
                    .ThenInclude(l => l.BookCopy)
                        .ThenInclude(c => c.Book)
            .Include(m => m.Loans)
                .ThenInclude(l => l.Status)
            .Include(m => m.Loans)
                .ThenInclude(l => l.BookCopy)
                    .ThenInclude(c => c.Book)
                        .ThenInclude(b => b.BookAuthors)
                            .ThenInclude(ba => ba.Author)
            .Include(m => m.Reservations)
                .ThenInclude(r => r.Status)
            .Include(m => m.Reservations)
                .ThenInclude(r => r.Book)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author);
    }

    private static MemberListDto MapToListDto(Member member)
    {
        return new MemberListDto
        {
            Id = member.Id,
            MemberNumber = member.MemberNumber,
            FullName = $"{member.User.FirstName} {member.User.LastName}".Trim(),
            Email = member.User.Email,
            Phone = member.User.PhoneNumber ?? member.MembershipApplication.PhoneNumber,
            PictureUrl = member.MembershipApplication.PictureUrl,
            Status = member.Status.Code,
            StatusName = member.Status.Name,
            UnpaidDebtAmount = member.Penalties.Where(p => !p.IsPaid).Sum(p => p.Amount)
        };
    }

    private static MemberDetailDto MapToDetailDto(Member member)
    {
        var list = MapToListDto(member);

        var activeLoans = member.Loans
            .Where(l => l.ReturnDate == null)
            .OrderByDescending(l => l.LoanDate)
            .Select(l =>
            {
                var isOverdue = l.DueDate.Date < DateTime.UtcNow.Date || l.Status.Code == Statuses.Loan.Overdue || l.Status.Code == Statuses.Loan.Critical;
                return new MemberLoanDto
                {
                    Id = l.Id,
                    BookTitle = l.BookCopy.Book.Title,
                    Authors = FormatAuthors(l.BookCopy.Book),
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = isOverdue ? Statuses.Loan.Overdue : l.Status.Code,
                    IsOverdue = isOverdue
                };
            })
            .ToList();

        var reservations = member.Reservations
            .Where(r => r.Status.Code == Statuses.Reservation.Waiting)
            .OrderBy(r => r.QueueNumber)
            .Select(r => new MemberReservationDto
            {
                Id = r.Id,
                BookTitle = r.Book.Title,
                Authors = FormatAuthors(r.Book),
                QueueNumber = r.QueueNumber,
                ReservationDate = r.ReservationDate,
                Status = r.Status.Code
            })
            .ToList();

        var penalties = member.Penalties
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new MemberPenaltyDto
            {
                Id = p.Id,
                Reason = p.PenaltyType.Name,
                RelatedBookTitle = p.Loan?.BookCopy.Book.Title,
                CreatedAt = p.CreatedAt,
                DelayDays = CalculateDelayDays(p),
                Amount = p.Amount,
                IsPaid = p.IsPaid
            })
            .ToList();

        return new MemberDetailDto
        {
            Id = list.Id,
            MemberNumber = list.MemberNumber,
            FullName = list.FullName,
            Email = list.Email,
            Phone = list.Phone,
            Address = member.User.Address ?? member.MembershipApplication.Address,
            PictureUrl = list.PictureUrl,
            Status = list.Status,
            StatusName = list.StatusName,
            MembershipType = member.MembershipApplication.MembershipType?.Name,
            RegistrationDate = member.CreatedAt,
            UnpaidDebtAmount = list.UnpaidDebtAmount,
            ActiveLoans = activeLoans,
            Reservations = reservations,
            Penalties = penalties
        };
    }

    private static string? FormatAuthors(Entity.Concrete.Catalog.Book book)
    {
        if (book.BookAuthors == null || book.BookAuthors.Count == 0)
            return null;

        return string.Join(", ", book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}".Trim()));
    }

    private static int CalculateDelayDays(Entity.Concrete.Operations.Penalty penalty)
    {
        if (penalty.Loan == null)
            return 0;

        var endDate = penalty.Loan.ReturnDate ?? DateTime.UtcNow;
        var days = (endDate.Date - penalty.Loan.DueDate.Date).Days;
        return days < 0 ? 0 : days;
    }

    private async Task<int> GetStatusIdByCodeAsync(string statusCode)
    {
        var statuses = await _statusRepository.FindAsync(x => x.Code == statusCode, tracking: false);
        return statuses.FirstOrDefault()?.Id ?? 0;
    }
}
