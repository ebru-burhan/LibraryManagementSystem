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

public class ReservationManager : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly IGenericRepository<Reservation> _reservationRepository;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<BookCopy> _bookCopyRepository;
    private readonly IGenericRepository<ReservationStatus> _reservationStatusRepository;

    public ReservationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;

        _reservationRepository = _unitOfWork.GetRepository<Reservation>();
        _memberRepository = _unitOfWork.GetRepository<Member>();
        _bookRepository = _unitOfWork.GetRepository<Book>();
        _bookCopyRepository = _unitOfWork.GetRepository<BookCopy>();
        _reservationStatusRepository = _unitOfWork.GetRepository<ReservationStatus>();
    }

    public async Task<IResult> CreateReservationAsync(int userId, CreateReservationDto dto)
    {


        var member = await _memberRepository.Query(tracking: false)
            .Include(m => m.Status)
            .Include(m => m.Penalties)
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return new ErrorResult("Üye bulunamadı.");

        if (member.Status.Code != Statuses.Member.Active)
            return new ErrorResult("Sadece aktif üyeler rezervasyon yapabilir.");

        var hasUnpaidPenalty = member.Penalties.Any(p => !p.IsPaid && !p.IsDeleted);
        if (hasUnpaidPenalty)
            return new ErrorResult("Ödenmemiş cezası bulunan üyeler rezervasyon yapamaz.");

        var book = await _bookRepository.Query(tracking: false)
            .FirstOrDefaultAsync(b => b.ExternalId == dto.BookExternalId);

        if (book == null)
            return new ErrorResult("Rezervasyon yapılmak istenen kitap bulunamadı.");

        var hasAvailableCopy = await _bookCopyRepository.Query(tracking: false)
            .Include(bc => bc.Status)
            .AnyAsync(bc => bc.BookId == book.Id && bc.Status.Code == Statuses.BookCopy.Available);

        if (hasAvailableCopy)
            return new ErrorResult("Bu kitabın rafta müsait bir kopyası bulunmaktadır. Rezervasyon yerine doğrudan ödünç alabilirsiniz.");

        var waitingStatus = await GetReservationStatusIdByCodeAsync(Statuses.Reservation.Waiting);

        var alreadyWaiting = await _reservationRepository.Query(tracking: false)
            .AnyAsync(r => r.BookId == book.Id && r.MemberId == member.Id && r.StatusId == waitingStatus);

        if (alreadyWaiting)
            return new ErrorResult("Bu kitap için zaten bekleyen bir rezervasyonunuz bulunmaktadır.");

        var currentQueueCount = await _reservationRepository.Query(tracking: false)
            .CountAsync(r => r.BookId == book.Id && r.StatusId == waitingStatus);

        var reservation = _mapper.Map<Reservation>(dto);

        reservation.MemberId = member.Id;
        reservation.BookId = book.Id;
        reservation.StatusId = waitingStatus;
        reservation.QueueNumber = currentQueueCount + 1;
        reservation.ReservationDate = DateTime.UtcNow;

        await _reservationRepository.AddAsync(reservation);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult($"Rezervasyon başarıyla oluşturuldu. Bu kitap için bekleme sıranız: {reservation.QueueNumber}");
    }

    public async Task<IDataResult<List<ReservationListDto>>> GetReservationsByMemberIdAsync(Guid memberExternalId)
    {
        var reservations = await _reservationRepository.Query(tracking: false)
            .Include(r => r.Book)
            .Include(r => r.Status)
            .Where(r => r.Member.ExternalId == memberExternalId)
            .OrderBy(r => r.QueueNumber)
            .ToListAsync();

        if (!reservations.Any())
            return new SuccessDataResult<List<ReservationListDto>>(new List<ReservationListDto>(), "Aktif bir rezervasyonunuz bulunmamaktadır.");

        var dtos = _mapper.Map<List<ReservationListDto>>(reservations);

        return new SuccessDataResult<List<ReservationListDto>>(dtos, "Rezervasyonlarınız başarıyla getirildi.");
    }

    ///Myprofile controller => Al sana Token'dan çıkan User.Id, bana rezervasyonları ver" der. ReservationManager ise arka planda önce Member tablosuna uğrar,
    ///üyenin gerçek kütüphane kimliğini (MemberId) bulur ve Reservation tablosundan o üyenin sıraya girdiği kitapları listeler.

    public async Task<IDataResult<List<ReservationListDto>>> GetReservationsByUserIdAsync(int userId)
    {
        // 1. KÖPRÜ: Token'dan gelen UserId ile Member (Üye) kaydını buluyoruz.
        var member = await _memberRepository.Query(tracking: false)
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return new ErrorDataResult<List<ReservationListDto>>("Sistemde aktif bir üyelik profiliniz bulunamadı.");

        // 2. ASIL İŞLEM: Bulduğumuz member.Id'yi kullanarak rezervasyonları çekiyoruz.
        var reservations = await _reservationRepository.Query(tracking: false)
            .Include(r => r.Book)
            .Include(r => r.Status)
            .Where(r => r.MemberId == member.Id) // Reservation tablosundaki MemberId ile eşleşme[cite: 12]
            .OrderBy(r => r.QueueNumber)
            .ToListAsync();

        if (!reservations.Any())
            return new SuccessDataResult<List<ReservationListDto>>(new List<ReservationListDto>(), "Aktif bir rezervasyonunuz bulunmamaktadır.");

        var dtos = _mapper.Map<List<ReservationListDto>>(reservations);

        return new SuccessDataResult<List<ReservationListDto>>(dtos, "Rezervasyonlarınız başarıyla getirildi.");
    }


    public async Task<IDataResult<List<ReservationListDto>>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.Query(tracking: false)
            .Include(r => r.Member)
                .ThenInclude(m => m.User)
            .Include(r => r.Book)
            .Include(r => r.Status)
            .OrderBy(r => r.QueueNumber)
            .ToListAsync();

        var dtos = _mapper.Map<List<ReservationListDto>>(reservations);

        return new SuccessDataResult<List<ReservationListDto>>(dtos, "Tüm rezervasyonlar başarıyla getirildi.");
    }


    public async Task<IResult> CancelReservationAsync(int userId, Guid reservationExternalId)
    {
        var member = await _unitOfWork.GetRepository<Member>().Query(tracking: false)
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return new ErrorResult("Üye profili bulunamadı.");

        var reservation = await _reservationRepository.Query(tracking: true)
            .FirstOrDefaultAsync(r => r.ExternalId == reservationExternalId && r.MemberId == member.Id);

        if (reservation == null)
            return new ErrorResult("Rezervasyon kaydı bulunamadı.");

        var waitingStatusId = await GetReservationStatusIdByCodeAsync(Statuses.Reservation.Waiting);
        if (reservation.StatusId != waitingStatusId)
            return new ErrorResult("Yalnızca 'Bekliyor' durumundaki aktif rezervasyonlar iptal edilebilir.");

        // İptal statüsüne çekiyoruz (Statülere Cancelled eklemediysek direkt silebilir veya Cancelled yapabiliriz)
        // Projende Cancelled statüsü varsa ID'sini bulup atayabilirsin, yoksa silebiliriz de:
        var cancelledStatusId = await GetReservationStatusIdByCodeAsync(Statuses.Reservation.Cancelled); // Varsa
        reservation.StatusId = cancelledStatusId;

        _reservationRepository.Update(reservation);

        // NOT: İsteğe bağlı olarak arkadaki kişilerin QueueNumber'larını 1 düşürmek 
        // enterprise projelerde yapılır ama staj projesi için statüyü iptal etmek fazlasıyla yeterlidir!

        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Rezervasyonunuz başarıyla iptal edildi.");
    }


    private async Task<int> GetReservationStatusIdByCodeAsync(string code)
    {
        var status = await _reservationStatusRepository.Query(tracking: false).FirstOrDefaultAsync(x => x.Code == code);
        return status?.Id ?? throw new InvalidOperationException($"Kritik Hata: '{code}' rezervasyon statüsü bulunamadı!");
    }
}