using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Catalog;
using Library.Model.Dtos.Catalog;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class BookManager : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Repository'leri konuştuğumuz gibi en tepeye, readonly olarak alıyoruz.
    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<Author> _authorRepository;

    public BookManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookRepository = _unitOfWork.GetRepository<Book>();
        _authorRepository = _unitOfWork.GetRepository<Author>();
    }

    public async Task<IDataResult<List<BookListDto>>> GetAllAsync()
    {
        try
        {
            var books = await _bookRepository.Query(tracking: false)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var dtos = _mapper.Map<List<BookListDto>>(books);

            return new SuccessDataResult<List<BookListDto>>(dtos, "Kitap listesi getirildi.");
        }
        catch (Exception ex)
        {
            // Hatayı doğrudan frontend'e mesaj olarak döneceğiz ki ekranda görebilelim
            return new ErrorDataResult<List<BookListDto>>($"PATLADI: {ex.Message} -> {ex.InnerException?.Message}");
        }
    }

    public async Task<IDataResult<BookDetailDto>> GetByIdAsync(Guid id)
    {
        // id parametresi artık ExternalId! Veritabanında PK (int) değil, ExternalId arıyoruz.
        var book = await _bookRepository.Query(tracking: false)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author) // Yazar alt DTO'sunu doldurmak için Include şart
            .FirstOrDefaultAsync(b => b.ExternalId == id);

        if (book == null)
            return new ErrorDataResult<BookDetailDto>("Kitap bulunamadı.");

        var dto = _mapper.Map<BookDetailDto>(book);

        return new SuccessDataResult<BookDetailDto>(dto, "Kitap detayı getirildi.");
    }

    public async Task<IResult> AddAsync(CreateBookDto dto)
    {
        // 1. DTO'yu Entity'ye çevir (Mapper burada ExternalId için Guid.NewGuid() üretecek)
        var book = _mapper.Map<Book>(dto);

        // 2. Güvenlik ve İlişki Duvarı: Gelen yazar Guid'lerini veritabanı int'lerine çevir
        if (dto.AuthorIds != null && dto.AuthorIds.Any())
        {
            // React'ten gelen Guid'leri (ExternalId) veritabanında arıyoruz
            var authors = await _authorRepository.Query(tracking: false)
                .Where(a => dto.AuthorIds.Contains(a.ExternalId))
                .ToListAsync();

            if (authors.Count != dto.AuthorIds.Count)
                return new ErrorResult("Seçilen yazarlardan bazıları sistemde bulunamadı.");

            // Bulunan yazarların 'int Id' değerlerini alıp ara tabloyu (BookAuthor) dolduruyoruz
            book.BookAuthors = authors.Select(a => new BookAuthor
            {
                AuthorId = a.Id
            }).ToList();
        }

        // 3. Kaydet
        await _bookRepository.AddAsync(book);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Kitap başarıyla eklendi.");
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var book = await _bookRepository.Query(tracking: true)
            .FirstOrDefaultAsync(b => b.ExternalId == id);

        if (book == null)
            return new ErrorResult("Kitap bulunamadı.");

        _bookRepository.Delete(book);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Kitap başarıyla silindi.");
    }
}