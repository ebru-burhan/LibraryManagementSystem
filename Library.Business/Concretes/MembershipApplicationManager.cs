using AutoMapper;
using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Membership;
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

    public async Task<IResult> CreateApplicationAsync(int userId, CreateMembershipApplicationDto dto)
    {
        // 1. KORUMA KALKANI: Hukuki onaylar (Frontend false gönderse bile backend reddeder)
        if (!dto.IsKvkkApproved || !dto.IsTermsAccepted)
        {
            return new ErrorResult("KVKK Aydınlatma Metni ve Kullanım Şartları onaylanmadan başvuru yapılamaz.");
        }

        var applicationRepository = _unitOfWork.GetRepository<MembershipApplication>();
        var userRepository = _unitOfWork.GetRepository<User>();

        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return new ErrorResult("Kullanıcı bulunamadı.");
        }


        // 2. KORUMA KALKANI: Spam/Tekrar Önleme (Idempotency)
        // Aynı kullanıcı daha önce başvuru yapmış mı? (SQL'e EXISTS sorgusu atar, hızlıdır)
        var hasExistingApp = await applicationRepository.FindAsync(x => x.UserId == userId);
        if (hasExistingApp.Any())
        {
            return new ErrorResult("Sistemde halihazırda bir başvurunuz bulunmaktadır. Lütfen onay sürecini bekleyiniz.");
        }

        // 3. KORUMA KALKANI: T.C. Kimlik Benzersizliği (Unique Constraint Patlamasını Önleme)
        // Manager içindeki TC kontrolünü senin repository'deki FindAsync metoduna[cite: 43] göre şöyle yaparız:
        var existingTcRecords = await applicationRepository.FindAsync(x => x.IdentityNumber == dto.IdentityNumber);
        if (existingTcRecords.Any())
        {
            return new ErrorResult("Bu T.C. Kimlik numarası ile sistemde zaten bir başvuru mevcut.");
        }

        // 4. VERİ DÖNÜŞÜMÜ   DTO'da Ad, Soyad, Email yok
        var application = new MembershipApplication
        {
            // Kullanıcıdan koparılan "Değiştirilemez" arşiv verileri (init korumalı)
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,

            // DTO'dan gelen veriler
            IdentityNumber = dto.IdentityNumber,
            // DateTime'dan DateOnly'ye Güvenli Çevirim
            DateOfBirth = DateOnly.FromDateTime(dto.DateOfBirth),
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            IsKvkkApproved = dto.IsKvkkApproved,
            IsTermsAccepted = dto.IsTermsAccepted,

            // Sistem atamaları
            UserId = userId,
            ApplicationStatusId = 1 // Pending
        };

        // 5. GÜVENLİ ATAMALAR EKSİK ALANLARI GÜVENLİ KAYNAKTAN (USER) MÜHÜRLE
        application.UserId = userId; // Token'dan gelen güvenilir ID
        application.ApplicationStatusId = 1; // Lookups tablosuna göre 'Pending / Bekliyor' durumu

        // 6. VERİTABANI İŞLEMİ
        await applicationRepository.AddAsync(application);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Başvurunuz başarıyla alındı ve onay sürecine girdi.");
    }
}