using Library.Model.Dtos.Membership;
using Library.Model.Results; // IResult ve Result için

namespace Library.Business.Abstracts;

public interface IMembershipApplicationService
{
    // Kullanıcı başvuru formunu doldurduğunda çalışacak metot

    Task<IDataResult<string>> CreateApplicationAsync(int userId, CreateMembershipApplicationDto dto);
    // IMembershipApplicationService.cs içerisine mevcut sözleşmelerin yanına eklenecek
    Task<IDataResult<MembershipApplicationDto>> GetByUserIdAsync(int userId);

    Task<IDataResult<List<MembershipApplicationDto>>> GetAllApplicationsDetailsAsync();

    Task<IDataResult<List<MembershipTypeDto>>> GetMembershipTypesAsync();

    Task<IResult> ApproveApplicationAsync(Guid applicationId);

    Task<IResult> RejectApplicationAsync(Guid applicationId);


    // İleride adminin başvuruları listeleyeceği metot
    // Task<IDataResult<List<MembershipApplicationDto>>> GetAllPendingApplicationsAsync();

    // İleride adminin başvuruyu onaylayacağı metot
    // Task<IResult> ApproveApplicationAsync(int applicationId, int adminId);
}