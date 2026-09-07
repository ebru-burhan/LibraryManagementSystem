using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Operations;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Constants;
using Library.Model.Dtos.Operations;
using Library.Model.Results;

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
    
        var members = await _memberRepository.FindAsync(
            m => m.ExternalId == createLoanDto.MemberId,
            m => m.Status,
            m => m.MembershipApplication.MembershipType, // Dinamik süre için gerekli
            m => m.Penalties);

        var member = members.FirstOrDefault();

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

        List<BookCopy> bookCopies;

        if (createLoanDto.BookCopyId != null && createLoanDto.BookCopyId != Guid.Empty)
        {
            bookCopies = (await _bookCopyRepository.FindAsync(
                c => c.ExternalId == createLoanDto.BookCopyId,
                c => c.Status)).ToList();
        }
        else
        {
            bookCopies = (await _bookCopyRepository.FindAsync(
                c => c.Barcode == createLoanDto.Barcode,
                c => c.Status)).ToList();
        }

        var bookCopy = bookCopies.FirstOrDefault();

        if (bookCopy == null)
            return new ErrorResult("Belirtilen kitap kopyası bulunamadı.");

        if (bookCopy.Status.Code != Statuses.BookCopy.Available)
            return new ErrorResult("Bu kitap şu anda rafta değil.");

        // 4. Dinamik Süre Hesaplama
        var membershipType = member.MembershipApplication.MembershipType;
        int maxLoanDays = membershipType != null ? membershipType.MaxLoanDays : 15; // Veritabanından gelen dinamik kural

        // 5. Veritabanındaki Statü ID'lerini Bulma
        var loanStatuses = await _loanStatusRepository.FindAsync(s => s.Code == Statuses.Loan.Borrowed, tracking: false);
        var borrowedStatus = loanStatuses.FirstOrDefault()
            ?? throw new Exception("Kritik Hata: 'BORROWED' ödünç statüsü bulunamadı.");

        var bookStatuses = await _bookStatusRepository.FindAsync(s => s.Code == Statuses.BookCopy.OnLoan, tracking: false);
        var onLoanStatus = bookStatuses.FirstOrDefault()
            ?? throw new Exception("Kritik Hata: 'ON_LOAN' kitap statüsü bulunamadı.");


        // uı den gelen dto da loandate varsa onu kullanrız yoksa ne zman butona basılırsa 
        var finalLoanDate = createLoanDto.LoanDate ?? DateTime.UtcNow;
        //beklenen teslim tarihi verilirse o yoksa entity de 15 dedik otomatik 
        var finalDueDate = createLoanDto.DueDate ?? finalLoanDate.AddDays(maxLoanDays);


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
}