using Library.Model.Dtos.Roles;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IRoleService
{
    Task<IResult> AddAsync(CreateRoleDto createRoleDto);
}