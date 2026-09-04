using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly IGenericRepository<MemberStatus> _statusRepository;

    public MemberManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memberRepository = _unitOfWork.GetRepository<Member>();
        _statusRepository = _unitOfWork.GetRepository<MemberStatus>();
    }

    public async Task<IDataResult<MemberDirectoryDto>> GetAllAsync(string? statusCode, string? search)
    {
        // Kodun bu kısmı senin yazdığınla tamamen aynı, değiştirmiyorum (Sadece GetAll)
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

        var items = _mapper.Map<List<MemberListDto>>(members);

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

    // DİKKAT: int id -> Guid id oldu. Ve m.Id yerine m.ExternalId arıyoruz!
    public async Task<IDataResult<MemberDetailDto>> GetByIdAsync(Guid id)
    {
        var member = await BuildMemberQuery(tracking: false)
            .FirstOrDefaultAsync(m => m.ExternalId == id); // ExternalId güvenlik duvarı!

        if (member == null)
            return new ErrorDataResult<MemberDetailDto>("Üye bulunamadı.");

        var detailDto = _mapper.Map<MemberDetailDto>(member);
        return new SuccessDataResult<MemberDetailDto>(detailDto, "Üye kartı getirildi.");
    }

    // DİKKAT: int id -> Guid id oldu.
    public async Task<IResult> UpdateStatusAsync(Guid id, string statusCode)
    {
        if (string.IsNullOrWhiteSpace(statusCode))
            return new ErrorResult("Üye durumu boş olamaz.");

        // Eski _memberRepository.GetByIdAsync() sadece PK (int) alıyordu.
        // Artık ExternalId'ye göre FindAsync ile buluyoruz.
        var memberList = await _memberRepository.FindAsync(m => m.ExternalId == id, tracking: true);
        var member = memberList.FirstOrDefault();

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

    // DİKKAT: int id -> Guid id oldu.
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var member = await BuildMemberQuery(tracking: true)
            .FirstOrDefaultAsync(m => m.ExternalId == id); // ExternalId güvenlik duvarı!

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


    /// private metotlar (Hiçbir değişiklik yok, kurgun zaten mükemmel)
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

    private async Task<int> GetStatusIdByCodeAsync(string statusCode)
    {
        var statuses = await _statusRepository.FindAsync(x => x.Code == statusCode, tracking: false);
        return statuses.FirstOrDefault()?.Id ?? 0;
    }
}