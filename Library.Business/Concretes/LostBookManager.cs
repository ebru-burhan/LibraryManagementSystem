using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Abstract;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Operations;
using Library.Entity.Constants;
using Library.Model.Dtos.Operations;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class LostBookManager : ILostBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LostBookManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult> ReportLostBookAsync(ReportLostBookDto dto)
    {
        // 1. İlgili Ödünç İşlemini Bul
        var loanRepository = _unitOfWork.GetRepository<Loan>();
        var loan = await loanRepository.Query(tracking: true)
            .Include(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.ExternalId == dto.LoanExternalId);

        if (loan == null)
            return new ErrorResult("Kayıp bildirimi yapılacak ödünç işlemi bulunamadı.");

        if (loan.ReturnDate.HasValue)
            return new ErrorResult("Bu kitap zaten iade edilmiş, kayıp bildirimi yapılamaz.");

        // 2. Ödüncü Kapat ve Kitap Statüsünü "LOST" Yap
        loan.ReturnDate = DateTime.UtcNow; // Gecikme faizini durdurmak için[cite: 37]

        var bookStatusId = await GetStatusIdAsync<BookStatus>(Statuses.BookCopy.Lost);
        loan.BookCopy.StatusId = bookStatusId;

        // 3. Kayıp Arşivine Ekle (LostBooks)
        var lostBook = new LostBook
        {
            MemberId = loan.MemberId,
            BookCopyId = loan.BookCopyId,
            BookValue = dto.BookValue, // Kütüphanecinin girdiği bedel
            DeclaredDate = DateTime.UtcNow,
            IsResolved = false // Tahsilat henüz yapılmadı[cite: 36]
        };
        await _unitOfWork.GetRepository<LostBook>().AddAsync(lostBook);

        // 4. Ceza (Penalty) Kes
        var penaltyTypeId = await GetStatusIdAsync<PenaltyType>(PenaltyTypes.Lost);
        var penalty = new Penalty
        {
            MemberId = loan.MemberId,
            LoanId = loan.Id, // Hangi ödünçten kaynaklandığı bağlanır[cite: 38]
            PenaltyTypeId = penaltyTypeId,
            Amount = dto.BookValue, // Ceza tutarı kitap bedeli kadardır[cite: 38]
            IsPaid = false
        };
        await _unitOfWork.GetRepository<Penalty>().AddAsync(penalty);

        // İşlemleri tek pakette veritabanına mühürle
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Kayıp kitap bildirimi başarıyla oluşturuldu ve ceza üyenin hesabına yansıtıldı.");
    }

    public async Task<IDataResult<LostBookKpiDto>> GetLostBookKpisAsync()
    {
        var repository = _unitOfWork.GetRepository<LostBook>();
        var query = repository.Query(tracking: false);

        var kpi = new LostBookKpiDto
        {
            TotalLostBooks = await query.CountAsync(),
            TotalUnpaidAmount = await query.Where(x => !x.IsResolved).SumAsync(x => x.BookValue),
            MonthlyReports = await query.Where(x => x.DeclaredDate.Month == DateTime.UtcNow.Month && x.DeclaredDate.Year == DateTime.UtcNow.Year).CountAsync()
        };

        return new SuccessDataResult<LostBookKpiDto>(kpi);
    }

    public async Task<IDataResult<List<LostBookListDto>>> GetAllLostBooksAsync()
    {
        var lostBooks = await _unitOfWork.GetRepository<LostBook>().Query(tracking: false)
            .Include(lb => lb.Member)
                .ThenInclude(m => m.User)
            .Include(lb => lb.BookCopy)
                .ThenInclude(bc => bc.Book)
            .OrderByDescending(lb => lb.DeclaredDate)
            .ToListAsync();

        // Artık AutoMapper şov yapıyor:
        var dtos = _mapper.Map<List<LostBookListDto>>(lostBooks);

        return new SuccessDataResult<List<LostBookListDto>>(dtos);
    }
    // Ortak Lookup ID Getirici
    private async Task<int> GetStatusIdAsync<T>(string code)
            where T : LookupEntity, IEntity
        {
            var entity = await _unitOfWork.GetRepository<T>()
                                          .Query(tracking: false)
                                          .FirstOrDefaultAsync(x => x.Code == code);

            return entity?.Id ?? throw new Exception($"Sistem Hatası: '{code}' statüsü bulunamadı.");
        }
}