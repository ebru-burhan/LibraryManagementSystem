using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Library.Model.Dtos.Roles;
using Library.Model.Results;

namespace Library.Business.Concretes;

public class RoleManager : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> AddAsync(CreateRoleDto createRoleDto)
    {
        // TODO: bunlarda auto mapping kullanayım şifre yok da ilk önce test edelim yetki girişi
        var role = new Role
        {
            Name = createRoleDto.Name,
            Description = createRoleDto.Description
        };

        await _unitOfWork.GetRepository<Role>().AddAsync(role);
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Rol başarıyla eklendi.");
    }
}