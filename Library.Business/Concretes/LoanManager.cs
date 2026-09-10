using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Operations;
using Library.Entity.Constants;
using Library.Model.Dtos.Operations;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class LoanManager : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly IGenericRepository<BookCopy> _bookCopyRepository;
    private readonly IGenericRepository<Penalty> _penaltyRepository;
    private readonly IGenericRepository<Reservation> _reservationRepository;

    public LoanManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        _loanRepository = _unitOfWork.GetRepository<Loan>();
        _memberRepository = _unitOfWork.GetRepository<Member>();
        _bookCopyRepository = _unitOfWork.GetRepository<BookCopy>();
        _penaltyRepository = _unitOfWork.GetRepository<Penalty>();
        _reservationRepository = _unitOfWork.GetRepository<Reservation>();
    }

    public async Task<IResult> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        var member = await _memberRepository.Query(tracking: false)
            .Include(m => m.Status)
            .Include(m => m.MembershipApplication)
                .ThenInclude(a => a.MembershipType)
            .Include(m => m.Penalties)
            .FirstOrDefaultAsync(m => m.ExternalId == createLoanDto.MemberId);

        if (member == null)
            return new ErrorResult("Üye bulunamadı.");

        if (member.Status.Code != Statuses.Member.Active)
            return new ErrorResult("Sadece aktif üyeler kitap ödünç alabilir.");

        var hasUnpaidPenalty = member.Penalties.Any(p => !p.IsPaid && !p.IsDeleted);
        if (hasUnpaidPenalty)
            return new ErrorResult("Ödenmemiş cezası bulunan üyeler yeni kitap alamaz.");

        if (createLoanDto.BookCopyId == null && string.IsNullOrWhiteSpace(createLoanDto.Barcode))
            return new ErrorResult("Lütfen ödünç verilecek kitap için bir ID veya Barkod sağlayın.");

        var query = _bookCopyRepository.Query(tracking: true)
            .Include(c => c.Status);

        BookCopy? bookCopy = null;

        if (createLoanDto.BookCopyId != null && createLoanDto.BookCopyId != Guid.Empty)
        {
            bookCopy = await query.FirstOrDefaultAsync(c => c.ExternalId == createLoanDto.BookCopyId);
        }
        else if (!string.IsNullOrWhiteSpace(createLoanDto.Barcode))
        {
            bookCopy = await query.FirstOrDefaultAsync(c => c.Barcode == createLoanDto.Barcode);
        }

        if (bookCopy == null)
            return new ErrorResult("Belirtilen kitap kopyası bulunamadı.");

        if (bookCopy.Status.Code != Statuses.BookCopy.Available)
            return new ErrorResult("Bu kitap şu anda rafta değil.");

        int maxLoanDays = 14;

        if (member.MembershipApplication?.MembershipType != null)
        {
            maxLoanDays = member.MembershipApplication.MembershipType.MaxLoanDays > 0
                ? member.MembershipApplication.MembershipType.MaxLoanDays
                : 14;
        }

        var borrowedStatusId = await GetLoanStatusIdByCodeAsync(Statuses.Loan.Borrowed);
        var onLoanStatusId = await GetBookStatusIdByCodeAsync(Statuses.BookCopy.OnLoan);

        var finalLoanDate = createLoanDto.LoanDate ?? DateTime.UtcNow;
        var finalDueDate = finalLoanDate.AddDays(maxLoanDays);

        var newLoan = new Loan
        {
            MemberId = member.Id,
            BookCopyId = bookCopy.Id,
            LoanDate = finalLoanDate,
            DueDate = finalDueDate,
            StatusId = borrowedStatusId
        };

        await _loanRepository.AddAsync(newLoan);

        bookCopy.StatusId = onLoanStatusId;
        _bookCopyRepository.Update(bookCopy);

        await _unitOfWork.CompleteAsync();

        return new SuccessResult($"Kitap başarıyla ödünç verildi. Teslim Tarihi: {newLoan.DueDate:dd.MM.yyyy}");
    }

    public async Task<IResult> ReturnLoanAsync(Guid loanExternalId, ReturnLoanDto dto)
    {
        var loan = await _loanRepository.Query(tracking: true)
            .Include(l => l.Member)
                .ThenInclude(m => m.MembershipApplication)
                    .ThenInclude(ma => ma.MembershipType)
            .Include(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.ExternalId == loanExternalId);

        if (loan == null)
            return new ErrorResult("İade edilmek istenen ödünç kaydı bulunamadı.");

        if (loan.ReturnDate.HasValue)
            return new ErrorResult("Bu kitap zaten iade edilmiş.");

        loan.ReturnDate = DateTime.UtcNow;

        var membershipType = loan.Member.MembershipApplication.MembershipType;
        int delayDays = (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;

        decimal totalPenaltyAmount = 0;
        var penaltyMessages = new List<string>();

        if (delayDays > membershipType.GracePeriodDays)
        {
            int penalizableDays = delayDays - membershipType.GracePeriodDays;
            decimal overdueAmount = penalizableDays * membershipType.DailyPenaltyRate;
            totalPenaltyAmount += overdueAmount;

            var penaltyTypeId = await GetPenaltyTypeIdByCodeAsync(PenaltyTypes.Overdue);

            var penalty = new Penalty
            {
                MemberId = loan.MemberId,
                LoanId = loan.Id,
                PenaltyTypeId = penaltyTypeId,
                Amount = overdueAmount,
                IsPaid = false
            };

            await _penaltyRepository.AddAsync(penalty);
            penaltyMessages.Add($"{delayDays} gün gecikme sebebiyle {overdueAmount:C2}");
        }

        if (dto.IsDamaged)
        {
            decimal damageAmount = dto.DamageAmount ?? 0;
            if (damageAmount <= 0)
                return new ErrorResult("Hasarlı kitaplar için lütfen geçerli bir hasar bedeli giriniz.");

            totalPenaltyAmount += damageAmount;

            var damagePenaltyTypeId = await GetPenaltyTypeIdByCodeAsync(PenaltyTypes.Damage);

            var damagePenalty = new Penalty
            {
                MemberId = loan.MemberId,
                LoanId = loan.Id,
                PenaltyTypeId = damagePenaltyTypeId,
                Amount = damageAmount,
                IsPaid = false
            };

            await _penaltyRepository.AddAsync(damagePenalty);
            penaltyMessages.Add($"hasar durumu sebebiyle {damageAmount:C2}");
        }

        var returnedLoanStatusId = await GetLoanStatusIdByCodeAsync(Statuses.Loan.Returned);

        string targetBookStatusCode;
        string reservationMessage = "";

        if (dto.IsDamaged)
        {
            targetBookStatusCode = Statuses.BookCopy.InRepair;
        }
        else
        {

            ///reservationnnn
            var waitingStatusId = await GetReservationStatusIdByCodeAsync(Statuses.Reservation.Waiting);

            var nextReservation = await _reservationRepository.Query(tracking: true)
                .Include(r => r.Member)
                    .ThenInclude(m => m.User)
                .Where(r => r.BookId == loan.BookCopy.BookId && r.StatusId == waitingStatusId)
                .OrderBy(r => r.QueueNumber)
                .FirstOrDefaultAsync();

            if (nextReservation != null)
            {
                targetBookStatusCode = Statuses.BookCopy.Reserved;

                var completedStatusId = await GetReservationStatusIdByCodeAsync(Statuses.Reservation.Completed);
                nextReservation.StatusId = completedStatusId;

                _reservationRepository.Update(nextReservation);

                reservationMessage = $" Dikkat: Bu kitap {nextReservation.Member.User.FirstName} {nextReservation.Member.User.LastName} adlı üye için ayrılmıştır.";
            }
            else
            {
                targetBookStatusCode = Statuses.BookCopy.Available;
            }
        }

        var targetBookStatusId = await GetBookStatusIdByCodeAsync(targetBookStatusCode);

        loan.StatusId = returnedLoanStatusId;
        loan.BookCopy.StatusId = targetBookStatusId;

        _loanRepository.Update(loan);
        await _unitOfWork.CompleteAsync();

        string resultMessage;
        if (totalPenaltyAmount > 0)
        {
            string penaltyDetail = string.Join(" ve ", penaltyMessages);
            string conditionText = dto.IsDamaged ? " Kitap tamir birimine (In Repair) yönlendirildi." : "";
            resultMessage = $"Kitap iade alındı. {penaltyDetail} tutarında ceza yansıtıldı.{conditionText}{reservationMessage}";
        }
        else
        {
            resultMessage = $"Kitap zamanında ve sorunsuz şekilde iade alındı. Teşekkür ederiz!{reservationMessage}";
        }

        return new SuccessResult(resultMessage);
    }

    public async Task<IDataResult<List<LoanListDto>>> GetActiveLoansAsync()
    {
        var loans = await _loanRepository.Query(tracking: false)
            .Include(l => l.Member)
                .ThenInclude(m => m.User)
            .Include(l => l.Member.MembershipApplication)
            .Include(l => l.BookCopy)
                .ThenInclude(bc => bc.Book)
            .Include(l => l.Status)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

        var listDtos = loans.Select(l => {
            var today = DateTime.UtcNow.Date;
            var dueDate = l.DueDate.Date;
            var diffDays = (dueDate - today).Days;

            bool isOverdue = diffDays < 0;
            int delayDays = isOverdue ? Math.Abs(diffDays) : 0;

            return new LoanListDto
            {
                Id = l.ExternalId,
                MemberFullName = l.Member?.User != null ? $"{l.Member.User.FirstName} {l.Member.User.LastName}" : "Bilinmiyor",
                MemberNumber = l.Member?.Id.ToString() ?? "LUM-000",
                BookTitle = l.BookCopy?.Book?.Title ?? "Bilinmiyor",
                Barcode = l.BookCopy?.Barcode ?? "",
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                Status = isOverdue ? "Gecikti" : (diffDays <= 2 ? "Kritik" : "Normal"),
                IsOverdue = isOverdue,
                DelayDays = delayDays
            };
        }).ToList();

        return new SuccessDataResult<List<LoanListDto>>(listDtos, "Aktif ödünçler başarıyla getirildi.");
    }



    public async Task<IDataResult<List<LoanListDto>>> GetLoansByUserIdAsync(int userId)
    {
        // 1. Token'dan gelen UserId ile üyeyi buluyoruz
        var member = await _memberRepository.Query(tracking: false)
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return new ErrorDataResult<List<LoanListDto>>("Sistemde aktif bir üyelik profiliniz bulunamadı.");

        // 2. Üyenin ödünç kayıtlarını ilişkileriyle birlikte çekiyoruz
        var loans = await _loanRepository.Query(tracking: false)
            .Include(l => l.Member)
                .ThenInclude(m => m.User)
            .Include(l => l.Member.MembershipApplication)
            .Include(l => l.BookCopy)
                .ThenInclude(bc => bc.Book)
            .Include(l => l.Status)
            .Where(l => l.MemberId == member.Id) // Sadece bu üyeye ait olanlar
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();

        // 3. Zaten mevcut olan LoanListDto mapping mantığımızı burada da çalıştırıyoruz
        var listDtos = loans.Select(l => {
            var today = DateTime.UtcNow.Date;
            var dueDate = l.DueDate.Date;
            var diffDays = (dueDate - today).Days;

            bool isOverdue = diffDays < 0;
            int delayDays = isOverdue ? Math.Abs(diffDays) : 0;

            return new LoanListDto
            {
                Id = l.ExternalId,
                MemberFullName = l.Member?.User != null ? $"{l.Member.User.FirstName} {l.Member.User.LastName}" : "Bilinmiyor",
                MemberNumber = l.Member?.Id.ToString() ?? "LUM-000",
                BookTitle = l.BookCopy?.Book?.Title ?? "Bilinmiyor",
                Barcode = l.BookCopy?.Barcode ?? "",
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                Status = l.ReturnDate != null ? "İade Edildi" : (isOverdue ? "Gecikti" : (diffDays <= 2 ? "Kritik" : "Normal")),
                IsOverdue = isOverdue,
                DelayDays = delayDays
            };
        }).ToList();

        return new SuccessDataResult<List<LoanListDto>>(listDtos, "Ödünç geçmişiniz başarıyla getirildi.");
    }

    // ADIM 4: YARDIMCI METOTLAR LOKAL ÇAĞRIM YAPIYOR (Constructor Şişmesini Engelliyor)
    private async Task<int> GetPenaltyTypeIdByCodeAsync(string code)
    {
        var type = await _unitOfWork.GetRepository<PenaltyType>().Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return type?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' ceza tipi bulunamadı!");
    }

    private async Task<int> GetLoanStatusIdByCodeAsync(string code)
    {
        var status = await _unitOfWork.GetRepository<LoanStatus>().Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' ödünç statüsü bulunamadı!");
    }

    private async Task<int> GetBookStatusIdByCodeAsync(string code)
    {
        var status = await _unitOfWork.GetRepository<BookStatus>().Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' kitap statüsü bulunamadı!");
    }

    // Eklemeyi unuttuğumuz metodumuz:
    private async Task<int> GetReservationStatusIdByCodeAsync(string code)
    {
        var status = await _unitOfWork.GetRepository<ReservationStatus>().Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' rezervasyon statüsü bulunamadı!");
    }
}