
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController] // Bu attribute, bunun bir API controller olduğunu ve JSON veri alışverişi yaptığını .NET'e kesin olarak bildirir.
public class UsersController : ControllerBase 
{
   

    public UsersController()
    {
       
    }

}