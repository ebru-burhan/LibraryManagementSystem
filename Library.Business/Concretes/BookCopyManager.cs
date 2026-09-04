using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;
using Library.Entity.Constants;
using Library.Model.Dtos.Catalog;

namespace Library.Business.Concretes;

public class BookCopyManager : IBookCopyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<BookCopy> _bookCopyRepository;
    private readonly IGenericRepository<BookStatus> _bookStatusRepository;
    private readonly IGenericRepository<Book> _bookRepository;

    public BookCopyManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookCopyRepository = _unitOfWork.GetRepository<BookCopy>();
        _bookStatusRepository = _unitOfWork.GetRepository<BookStatus>();
        _bookRepository = _unitOfWork.GetRepository<Book>();
    }
    public async Task<IResult> CreateBookCopyAsync(CreateBookCopyDto dto)
    {
        try
        {
            var book = await _bookRepository
                .Query(tracking: false)
                .FirstOrDefaultAsync(x => x.ExternalId == dto.BookId);

            if (book == null)
                return new ErrorResult("İlişkilendirilecek kitap sistemde bulunamadı.");

            var availableStatus = await _bookStatusRepository
                .Query(tracking: false)
                .FirstOrDefaultAsync(x => x.Code == Statuses.BookCopy.Available); // Code üzerinden arıyoruz unutma

            if (availableStatus == null)
                return new ErrorResult("Sistemde 'AVAILABLE' statüsü bulunamadı.");

            var bookCopy = _mapper.Map<BookCopy>(dto);
            bookCopy.BookId = book.Id;
            bookCopy.StatusId = availableStatus.Id;

            await _bookCopyRepository.AddAsync(bookCopy);

            // Hata tam olarak burada, veritabanına kaydederken patlıyor!
            await _unitOfWork.CompleteAsync();

            return new SuccessResult("Book copy başarıyla eklendi");
        }
        catch (Exception ex)
        {
            // PATLAYAN HATAYI YAKALAYIP FRONTEND'E GÖNDERİYORUZ!
            string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return new ErrorResult($"SİSTEM HATASI: {errorMessage}");
        }
    }

    // 2. LİSTELEME İŞLEMİ (Include zinciri ile ilişkili verileri dolduruyoruz)
    public async Task<IDataResult<List<BookCopyListDto>>> GetAllBookCopiesAsync()
    {
        var bookCopies = await _bookCopyRepository
            .Query(tracking: false)
            .Include(bc => bc.Status) // Statü adı için
            .Include(bc => bc.Book)   // Kitap bilgisi için
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author) // Yazarlar için
            .Include(bc => bc.Book)
                .ThenInclude(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category) // Kategoriler için
            .ToListAsync();

        var dtos = _mapper.Map<List<BookCopyListDto>>(bookCopies);
        return new SuccessDataResult<List<BookCopyListDto>>(dtos, "Kitap kopyaları başarıyla getirildi.");
    }

    // 3. DETAY İŞLEMİ (Guid ExternalId ile arama)
    public async Task<IDataResult<BookCopyDetailDto>> GetBookCopyByExternalIdAsync(Guid externalId)
    {
        var bookCopy = await _bookCopyRepository
            .Query(tracking: false)
            .Include(bc => bc.Status)
            .Include(bc => bc.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .Include(bc => bc.Book)
                .ThenInclude(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(x => x.ExternalId == externalId);

        if (bookCopy == null)
        {
            return new ErrorDataResult<BookCopyDetailDto>("Aradığınız kitap kopyası bulunamadı.");
        }

        var dto = _mapper.Map<BookCopyDetailDto>(bookCopy);
        return new SuccessDataResult<BookCopyDetailDto>(dto, "Kitap kopyası detayı getirildi.");
    }
}