using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Constants;
using Library.Model.Dtos.Membership;
using Library.Model.Results;

namespace Library.Business.Concretes;

public class MembershipApplicationManager : IMembershipApplicationService
{
    private readonly IUnitOfWork _unitOfWork;

    public MembershipApplicationManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDataResult<string>> CreateApplicationAsync(int userId, CreateMembershipApplicationDto dto)
    {
        // 1. İş Kuralları ve Kullanıcıyı Getir
        var businessResult = await CheckBusinessRulesAsync(userId, dto.IdentityNumber);
        if (!businessResult.Success)
        {
            return new ErrorDataResult<string>(businessResult.Message);
        }

        // 2. Lookup Çözümleme
        var pendingStatusId = await GetPendingStatusIdAsync();
        if (pendingStatusId == 0)
        {
            return new ErrorDataResult<string>("Sistemde başvuru durumları tanımlanmamış.");
        }

        var user = businessResult.Data; // Validasyondan geçen kullanıcı verisi

        // 3. Object Initializer (init-only kısıtlamasını aşmak için tek seferde atama)
        var application = new MembershipApplication
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IdentityNumber = dto.IdentityNumber,
            DateOfBirth = DateOnly.FromDateTime(dto.DateOfBirth),
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            UserId = userId,
            ApplicationStatusId = pendingStatusId
        };

        // 4. Veri Tabanı İşlemi
        await _unitOfWork.GetRepository<MembershipApplication>().AddAsync(application);
        await _unitOfWork.CompleteAsync();

        return new SuccessDataResult<string>(Statuses.MembershipApplication.Pending, "Başvurunuz başarıyla alındı.");
    }





    public async Task<IDataResult<MembershipApplicationDto>> GetByUserIdAsync(int userId)
    {
        var applicationRepository = _unitOfWork.GetRepository<MembershipApplication>();

        // Eğer Generic Repository'de Include desteğin varsa ilişkiyi doğrudan dahil edebilirsin:
        // var applications = await applicationRepository.FindWithIncludeAsync(x => x.UserId == userId, x => x.ApplicationStatus);

        var applications = await applicationRepository.FindAsync(x => x.UserId == userId);
        var application = applications.OrderByDescending(x => x.Id).FirstOrDefault();

        if (application == null)
        {
            return new ErrorDataResult<MembershipApplicationDto>("Kullanıcıya ait başvuru bulunamadı.");
        }

        // Eğer Include kullanmadıysan, Lookup tablosundan statü kodunu güvenle çekiyoruz:
        var statusRepository = _unitOfWork.GetRepository<MembershipApplicationStatus>();
        var status = await statusRepository.GetByIdAsync(application.ApplicationStatusId);

        // Sabitlerden gelen varsayılan statü
        string statusCode = status?.Code ?? Statuses.MembershipApplication.Pending;

        var dto = new MembershipApplicationDto
        {
            Id = application.Id,
            ApplicationStatus = statusCode // "PENDING", "APPROVED" vb.
        };

        return new SuccessDataResult<MembershipApplicationDto>(dto, "Başvuru durumu getirildi.");
    }



    private async Task<IDataResult<User>> CheckBusinessRulesAsync(int userId, string identityNumber)
    {
        var userRepository = _unitOfWork.GetRepository<User>();
        var applicationRepository = _unitOfWork.GetRepository<MembershipApplication>();

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) return new ErrorDataResult<User>("Kullanıcı bulunamadı.");

        var hasExistingApp = await applicationRepository.FindAsync(x => x.UserId == userId);
        if (hasExistingApp.Any()) return new ErrorDataResult<User>("Sistemde halihazırda bir başvurunuz bulunmaktadır.");

        var existingTcRecords = await applicationRepository.FindAsync(x => x.IdentityNumber == identityNumber);
        if (existingTcRecords.Any()) return new ErrorDataResult<User>("Bu T.C. Kimlik numarası ile sistemde zaten bir başvuru mevcut.");

        return new SuccessDataResult<User>(user);
    }

    private async Task<int> GetPendingStatusIdAsync()
    {
        var statusRepository = _unitOfWork.GetRepository<MembershipApplicationStatus>();
        var pendingStatuses = await statusRepository.FindAsync(x => x.Code == Statuses.MembershipApplication.Pending);
        return pendingStatuses.FirstOrDefault()?.Id ?? 0;
    }
}