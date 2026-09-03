using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;
using Library.Entity.Constants;

namespace Library.Business.Concretes;

public class BookCopyManager : IBookCopyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<BookCopy> _bookCopyRepository;
    private readonly IGenericRepository<BookStatus> _bookStatusRepository;

    public BookCopyManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookCopyRepository = _unitOfWork.GetRepository<BookCopy>();
        _bookStatusRepository = _unitOfWork.GetRepository<BookStatus>();
    }

    public async Task<IResult> CreateBookCopyAsync(CreateBookCopyDto dto)
    {
        var availableStatus = await _bookStatusRepository
        .Query(tracking: false)
        .FirstOrDefaultAsync(x => x.Name == Statuses.BookCopy.Available);

        if (availableStatus == null)
        {
            return new ErrorResult("Sistemde 'AVAILABLE' statüsü bulunamadı. Lütfen Lookup tablolarını kontrol edin.");
        }

        // AutoMapper ile listeyi tek satırda dönüştürüyoruz
        var bookCopy = _mapper.Map<BookCopy>(dto);
            bookCopy.StatusId = availableStatus.Id;
        // 2. Statüyü varsayılan olarak "AVAILABLE" yap
        // Not: Eğer Status tablon varsa, veritabanından Name == Statuses.BookCopy.Available olanın Id'sini bulup buraya atayabilirsin.

        // 3. Veritabanına kaydet
        await _bookCopyRepository.AddAsync(bookCopy);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Book copy başarıyla eklendi");
    }
}