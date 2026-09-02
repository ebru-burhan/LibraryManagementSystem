using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Constants;
using Library.Model.Dtos.Membership;
using Library.Model.Results;
using Microsoft.EntityFrameworkCore;

namespace Library.Business.Concretes;

public class MembershipApplicationManager : IMembershipApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Sık kullanılan Repository'leri Constructor'da tanımlıyoruz (Global Repository'ler)
    private readonly IGenericRepository<MembershipApplication> _applicationRepository;
    private readonly IGenericRepository<MembershipApplicationStatus> _statusRepository;

    public MembershipApplicationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;

        // Repository'leri sadece bir kez burada oluşturuyoruz
        _applicationRepository = _unitOfWork.GetRepository<MembershipApplication>();
        _statusRepository = _unitOfWork.GetRepository<MembershipApplicationStatus>();
    }

    public async Task<IDataResult<string>> CreateApplicationAsync(int userId, CreateMembershipApplicationDto dto)
    {
        var businessResult = await CheckBusinessRulesAsync(userId, dto.IdentityNumber);
        if (!businessResult.Success)
            return new ErrorDataResult<string>(businessResult.Message);

        var pendingStatusId = await GetStatusIdByCodeAsync(Statuses.MembershipApplication.Pending);

        if (string.IsNullOrWhiteSpace(dto.MembershipTypeCode))
            return new ErrorDataResult<string>("Üyelik türü seçilmeden başvuru yapılamaz.");

        var membershipTypeId = await GetMembershipTypeIdByCodeAsync(dto.MembershipTypeCode);

        var user = businessResult.Data;

        var application = new MembershipApplication
        {
           
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IdentityNumber = dto.IdentityNumber,
            DateOfBirth = DateOnly.FromDateTime(dto.DateOfBirth),
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            PictureUrl = dto.PictureUrl,
            DocumentUrl = dto.DocumentUrl,
            UserId = userId,
            ApplicationStatusId = pendingStatusId,
            MembershipTypeId = membershipTypeId
        };

        await _applicationRepository.AddAsync(application);
        await _unitOfWork.CompleteAsync();

        return new SuccessDataResult<string>(Statuses.MembershipApplication.Pending, "Başvurunuz başarıyla alındı.");
    }

    public async Task<IDataResult<MembershipApplicationDto>> GetByUserIdAsync(int userId)
    {
        var applications = await _applicationRepository.FindAsync(x => x.UserId == userId);
        var application = applications.OrderByDescending(x => x.Id).FirstOrDefault();

        if (application == null)
            return new ErrorDataResult<MembershipApplicationDto>("Kullanıcıya ait başvuru bulunamadı.");

        var status = await _statusRepository.GetByIdAsync(application.ApplicationStatusId);
        string statusCode = status?.Code ?? Statuses.MembershipApplication.Pending;

        var dto = new MembershipApplicationDto
        {
            Id = application.Id,
            ApplicationStatus = statusCode
        };

        return new SuccessDataResult<MembershipApplicationDto>(dto, "Başvuru durumu getirildi.");
    }

    public async Task<IDataResult<List<MembershipApplicationDto>>> GetAllApplicationsDetailsAsync()
    {
        var applications = await _applicationRepository.FindAsync(
            x => true,
            x => x.ApplicationStatus,
            x => x.MembershipType);

        if (applications == null || !applications.Any())
            return new ErrorDataResult<List<MembershipApplicationDto>>("Hiç başvuru bulunamadı.");

        var applicationDtos = _mapper.Map<List<MembershipApplicationDto>>(applications);

        return new SuccessDataResult<List<MembershipApplicationDto>>(applicationDtos, "Başvuru listesi başarıyla getirildi.");
    }

    public async Task<IDataResult<List<MembershipTypeDto>>> GetMembershipTypesAsync()
    {
        var typeRepository = _unitOfWork.GetRepository<MembershipType>();
        var types = await typeRepository.GetAllAsync(tracking: false);

        if (types == null || !types.Any())
            return new ErrorDataResult<List<MembershipTypeDto>>("Üyelik türleri bulunamadı.");

        var dtos = types
            .OrderBy(x => x.Name)
            .Select(x => new MembershipTypeDto
            {
                Code = x.Code,
                Name = x.Name
            })
            .ToList();

        return new SuccessDataResult<List<MembershipTypeDto>>(dtos);
    }

    public async Task<IResult> ApproveApplicationAsync(int applicationId)
    {
        var application = await _applicationRepository.GetByIdAsync(applicationId);
        if (application == null) return new ErrorResult("Belirtilen başvuru bulunamadı.");

        var approvedStatusId = await GetStatusIdByCodeAsync(Statuses.MembershipApplication.Approved);
        if (approvedStatusId == 0) return new ErrorResult("Sistemde 'Onaylandı' statüsü bulunamadı.");

        if (application.ApplicationStatusId == approvedStatusId)
            return new ErrorResult("Bu başvuru zaten onaylanmış.");

        application.ApplicationStatusId = approvedStatusId;
        _applicationRepository.Update(application);

        var memberRepository = _unitOfWork.GetRepository<Member>();
        var newMember = new Member
        {
            UserId = application.UserId,
            MembershipApplicationId = application.Id,
            MemberNumber = $"LUM-{DateTime.Now.Year}-{application.UserId:D3}",
            IsActive = true
        };
        await memberRepository.AddAsync(newMember);

        var roleRepository = _unitOfWork.GetRepository<Role>();
        var memberRole = (await roleRepository.FindAsync(x => x.Name == "Member")).FirstOrDefault();

        if (memberRole != null)
        {
            var userRoleRepository = _unitOfWork.GetRepository<UserRole>();
            var hasRole = (await userRoleRepository.FindAsync(x => x.UserId == application.UserId && x.RoleId == memberRole.Id)).Any();
            if (!hasRole)
            {
                await userRoleRepository.AddAsync(new UserRole
                {
                    UserId = application.UserId,
                    RoleId = memberRole.Id
                });
            }
        }

        await _unitOfWork.CompleteAsync();
        return new SuccessResult("Başvuru başarıyla onaylandı ve kütüphane üyeliği oluşturuldu.");
    }

    public async Task<IResult> RejectApplicationAsync(int applicationId)
    {
        var application = await _applicationRepository.GetByIdAsync(applicationId);
        if (application == null) return new ErrorResult("Belirtilen başvuru bulunamadı.");

        var rejectedStatusId = await GetStatusIdByCodeAsync(Statuses.MembershipApplication.Rejected);
        if (rejectedStatusId == 0) return new ErrorResult("Sistemde 'Reddedildi' statüsü bulunamadı.");

        if (application.ApplicationStatusId == rejectedStatusId)
            return new ErrorResult("Bu başvuru zaten reddedilmiş durumda.");

        application.ApplicationStatusId = rejectedStatusId;
        _applicationRepository.Update(application);

        await _unitOfWork.CompleteAsync();
        return new SuccessResult("Başvuru başarıyla reddedildi.");
    }

    // Private Metotlar ---------------------------------------------

    private async Task<IDataResult<User>> CheckBusinessRulesAsync(int userId, string identityNumber)
    {
        var userRepository = _unitOfWork.GetRepository<User>();

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) return new ErrorDataResult<User>("Kullanıcı bulunamadı.");

        var hasExistingApp = await _applicationRepository.FindAsync(x => x.UserId == userId);
        if (hasExistingApp.Any()) return new ErrorDataResult<User>("Sistemde halihazırda bir başvurunuz bulunmaktadır.");

        var existingTcRecords = await _applicationRepository.FindAsync(x => x.IdentityNumber == identityNumber);
        if (existingTcRecords.Any()) return new ErrorDataResult<User>("Bu T.C. Kimlik numarası ile sistemde zaten bir başvuru mevcut.");

        return new SuccessDataResult<User>(user);
    }

    // Ortak Status Getirme Metodu (Sadece istenen kodu alır, ID döner)
    private async Task<int> GetStatusIdByCodeAsync(string statusCode)
    {
        var statuses = await _statusRepository.FindAsync(x => x.Code == statusCode);
        var status = statuses.FirstOrDefault();

        if (status == null)
        {
            // 0 dönmek yerine doğrudan hata fırlatıyoruz
            throw new Exception($"Kritik Hata: '{statusCode}' statüsü veritabanında bulunamadı!");
        }

        return status.Id;
    }

    private async Task<int> GetMembershipTypeIdByCodeAsync(string membershipTypeCode)
    {
        var normalizedCode = membershipTypeCode.Trim().ToUpperInvariant();
        var typeRepository = _unitOfWork.GetRepository<MembershipType>();
        var types = await typeRepository.FindAsync(x => x.Code == normalizedCode, tracking: false);
        var membershipType = types.FirstOrDefault();

        if (membershipType == null)
            throw new InvalidOperationException($"Kritik Hata: '{normalizedCode}' üyelik türü veritabanında bulunamadı!");

        return membershipType.Id;
    }
}