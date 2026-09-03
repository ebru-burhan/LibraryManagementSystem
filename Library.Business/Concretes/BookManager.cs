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
    private readonly IGenericRepository<Book> _bookRepository;

    public BookManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookRepository = _unitOfWork.GetRepository<Book>();
    }




    public async Task<IDataResult<List<BookListDto>>> GetAllBooksAsync()
    {

        // Merkezi metottan sorguyu alıyoruz
        var query = BuildListQuery(tracking: false);


        // İleride buraya "if (!string.IsNullOrWhiteSpace(search)) query = query.Where(...)" gelecek filtre de yapcaz bunu

        var books = await query.ToListAsync();

        var bookDtos = _mapper.Map<List<BookListDto>>(books);

        return new SuccessDataResult<List<BookListDto>>(bookDtos, "Kitaplar başarıyla listelendi.");
    }





    private IQueryable<Book> BuildListQuery(bool tracking = false)
    {
        return _bookRepository.Query(tracking)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author);
    }
}
   