using AutoMapper;
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
    private readonly IGenericRepository<BookStatus> _bookStatusRepository;
    private readonly IGenericRepository<LoanStatus> _loanStatusRepository;

    public LoanManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _loanRepository = _unitOfWork.GetRepository<Loan>();
        _memberRepository = _unitOfWork.GetRepository<Member>();
        _bookCopyRepository = _unitOfWork.GetRepository<BookCopy>();
        _bookStatusRepository = _unitOfWork.GetRepository<BookStatus>();
        _loanStatusRepository = _unitOfWork.GetRepository<LoanStatus>();
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

        // 2. Ödenmemiş Ceza Kontrolü
        var hasUnpaidPenalty = member.Penalties.Any(p => !p.IsPaid && !p.IsDeleted);
        if (hasUnpaidPenalty)
            return new ErrorResult("Ödenmemiş cezası bulunan üyeler yeni kitap alamaz.");

        if (createLoanDto.BookCopyId == null && string.IsNullOrWhiteSpace(createLoanDto.Barcode))
            return new ErrorResult("Lütfen ödünç verilecek kitap için bir ID veya Barkod sağlayın.");

        // Query ve Include gücüyle hem takibi (tracking) açıyoruz hem status'ü çekiyoruz
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

      // Süre Hesaplama
        int maxLoanDays = 14; // Varsayılan 

        if (member.MembershipApplication?.MembershipType != null)
        {
            // Eğer üyelik tipinde MaxLoanDays tanımlıysa onu al, yoksa varsayılanı kullan
            maxLoanDays = member.MembershipApplication.MembershipType.MaxLoanDays > 0
                ? member.MembershipApplication.MembershipType.MaxLoanDays
                : 14;
        }

        // 5. Veritabanındaki Statü ID'lerini Bulma
        var loanStatuses = await _loanStatusRepository.FindAsync(s => s.Code == Statuses.Loan.Borrowed, tracking: false);
        var borrowedStatus = loanStatuses.FirstOrDefault()
            ?? throw new Exception("Kritik Hata: 'BORROWED' ödünç statüsü bulunamadı.");

        var bookStatuses = await _bookStatusRepository.FindAsync(s => s.Code == Statuses.BookCopy.OnLoan, tracking: false);
        var onLoanStatus = bookStatuses.FirstOrDefault()
            ?? throw new Exception("Kritik Hata: 'ON_LOAN' kitap statüsü bulunamadı.");


        // uı den gelen dto da loandate varsa onu kullanrız yoksa ne zman butona basılırsa 
        var finalLoanDate = createLoanDto.LoanDate ?? DateTime.UtcNow;
        //beklenen teslim tarihi verilirse o yoksa membertype a göre belirledik 
        var finalDueDate = finalLoanDate.AddDays(maxLoanDays); // Artık 14 veya 30 gün eklenecek!


        // 6. Kayıtları Oluşturma ve Güncelleme
        var newLoan = new Loan
        {
            MemberId = member.Id, // İçeride (int) kimlikleri bağlıyoruz
            BookCopyId = bookCopy.Id,
            LoanDate = finalLoanDate,
            DueDate = finalDueDate,
            StatusId = borrowedStatus.Id
        };

        await _loanRepository.AddAsync(newLoan);

        bookCopy.StatusId = onLoanStatus.Id;
        _bookCopyRepository.Update(bookCopy);

        // 7. Unit of Work Şovu (İşlem Bütünlüğü - Transaction)
        await _unitOfWork.CompleteAsync();

        return new SuccessResult($"Kitap başarıyla ödünç verildi. Teslim Tarihi: {newLoan.DueDate:dd.MM.yyyy}");
    }

    public async Task<IResult> ReturnLoanAsync(Guid loanExternalId)
    {
        // 1. İlgili ödünç kaydını Üye, Üyelik Tipi ve Kitap Kopyası ile birlikte getir
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

        // 2. İade işlemini gerçekleştir
        loan.ReturnDate = DateTime.UtcNow;

        // 3. Gecikme ve Ceza Hesaplama Motoru
        var membershipType = loan.Member.MembershipApplication.MembershipType;
        int delayDays = (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;

        decimal penaltyAmount = 0;

        // Tolerans süresi (GracePeriodDays) aşıldıysa ceza kesilir
        if (delayDays > membershipType.GracePeriodDays)
        {
            // Tolerans süresini düşerek adil bir ceza hesaplaması yapıyoruz
            int penalizableDays = delayDays - membershipType.GracePeriodDays;
            penaltyAmount = penalizableDays * membershipType.DailyPenaltyRate;

            var penaltyTypeId = await GetPenaltyTypeIdByCodeAsync(PenaltyTypes.Overdue);

            var penalty = new Penalty
            {
                MemberId = loan.MemberId,
                LoanId = loan.Id,
                PenaltyTypeId = penaltyTypeId,
                Amount = penaltyAmount,
                IsPaid = false
                // CreatedAt ve ExternalId, BaseEntity/SaveChanges interceptor'dan otomatik gelmiyorsa burada set edebilirsin
            };

            await _unitOfWork.GetRepository<Penalty>().AddAsync(penalty);
        }

        // 4. Statüleri Güncelle (Lookup tablolarından kodlara göre ID'leri çek)
        var returnedLoanStatusId = await GetLoanStatusIdByCodeAsync(Statuses.Loan.Returned);
        var availableBookStatusId = await GetBookStatusIdByCodeAsync(Statuses.BookCopy.Available);

        loan.StatusId = returnedLoanStatusId;
        loan.BookCopy.StatusId = availableBookStatusId; // Kitap tekrar rafa dönüyor

        _loanRepository.Update(loan);
        await _unitOfWork.CompleteAsync();

        // 5. Dinamik Sonuç Mesajı
        var resultMessage = penaltyAmount > 0
            ? $"Kitap başarıyla iade alındı. {delayDays} gün gecikme sebebiyle {penaltyAmount:C2} tutarında ceza yansıtıldı."
            : "Kitap zamanında ve sorunsuz şekilde iade alındı. Teşekkür ederiz!";

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
            .Where(l => l.ReturnDate == null) // Henüz iade edilmemiş aktif ödünçler
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






    ///offf kod tekrarı bunları başka yerde de getirdik
    ///TODO: IlookupService desek olur toparlarız !!!!!!!!!!!!!!

    private async Task<int> GetPenaltyTypeIdByCodeAsync(string code)
    {
        var repository = _unitOfWork.GetRepository<PenaltyType>();
        var type = await repository.Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return type?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' ceza tipi bulunamadı!");
    }

    private async Task<int> GetLoanStatusIdByCodeAsync(string code)
    {
        var repository = _unitOfWork.GetRepository<LoanStatus>();
        var status = await repository.Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' ödünç statüsü bulunamadı!");
    }

    private async Task<int> GetBookStatusIdByCodeAsync(string code)
    {
        var repository = _unitOfWork.GetRepository<BookStatus>();
        var status = await repository.Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' kitap statüsü bulunamadı!");
    }
}