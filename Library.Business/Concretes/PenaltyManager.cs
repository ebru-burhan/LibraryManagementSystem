using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Operations;
using Library.Entity.Constants;
using Library.Model.Dtos.Penalties;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class PenaltyManager : IPenaltyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Penalty> _penaltyRepository;

    public PenaltyManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _penaltyRepository = _unitOfWork.GetRepository<Penalty>();
    }

    public async Task<IDataResult<List<PenaltyListDto>>> GetPenaltiesByMemberIdAsync(Guid memberExternalId)
    {
        var penalties = await _penaltyRepository.Query(tracking: false)
            .Include(p => p.PenaltyType)
            .Include(p => p.Loan)
                .ThenInclude(l => l!.BookCopy)
                    .ThenInclude(c => c.Book)
            .Where(p => p.Member.ExternalId == memberExternalId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        if (penalties == null || !penalties.Any())
            return new SuccessDataResult<List<PenaltyListDto>>(new List<PenaltyListDto>(), "Üyeye ait ceza kaydı bulunmuyor.");

        var dtos = _mapper.Map<List<PenaltyListDto>>(penalties);
        return new SuccessDataResult<List<PenaltyListDto>>(dtos);
    }

    public async Task<IResult> PayPenaltyAsync(Guid penaltyExternalId)
    {

        var penalty = await _penaltyRepository.Query(tracking: true)
            .Include(p => p.PenaltyType)
            .Include(p => p.Loan)
            .FirstOrDefaultAsync(p => p.ExternalId == penaltyExternalId);

        if (penalty == null)
            return new ErrorResult("Ceza kaydı bulunamadı.");

        if (penalty.IsPaid)
            return new ErrorResult("Bu ceza zaten ödenmiş.");

       
        penalty.IsPaid = true;
        penalty.PaidDate = DateTime.UtcNow;

        _penaltyRepository.Update(penalty);

        
        // ödenince durumu değiştirmem gerek. lost için off bunu burda sevmedim de
        if (penalty.PenaltyType.Code == PenaltyTypes.Lost && penalty.LoanId.HasValue)
        {
            var lostBookRepo = _unitOfWork.GetRepository<LostBook>();

            // Bu kitaba ait henüz çözülmemiş (IsResolved = false) kayıp kaydını bul
            var lostBook = await lostBookRepo.Query(tracking: true)
                .FirstOrDefaultAsync(lb => lb.BookCopyId == penalty.Loan!.BookCopyId && !lb.IsResolved);

            if (lostBook != null)
            {
                lostBook.IsResolved = true;
                lostBookRepo.Update(lostBook);
            }
        }


        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Ceza tahsilatı başarıyla gerçekleştirildi.");
    }
}