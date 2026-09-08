using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Operations;
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
            .FirstOrDefaultAsync(p => p.ExternalId == penaltyExternalId);

        if (penalty == null)
            return new ErrorResult("Ceza kaydı bulunamadı.");

        if (penalty.IsPaid)
            return new ErrorResult("Bu ceza zaten ödenmiş.");

        // Ödeme işlemi kuralları
        penalty.IsPaid = true;
        penalty.PaidDate = DateTime.UtcNow;

        _penaltyRepository.Update(penalty);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Ceza tahsilatı başarıyla gerçekleştirildi.");
    }
}