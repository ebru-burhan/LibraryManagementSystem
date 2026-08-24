using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController] // Bu attribute, bunun bir API controller olduğunu ve JSON veri alışverişi yaptığını .NET'e kesin olarak bildirir.
public class UsersController : ControllerBase // Dikkat: Controller değil, ControllerBase!
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _unitOfWork.GetRepository<User>().GetAllAsync(tracking: false);

        var userList = users.Select(u => new
        {
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.PhoneNumber,
            u.IsActive
        });

        return Ok(userList);
    }
}