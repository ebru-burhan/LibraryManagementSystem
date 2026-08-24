using Library.Business.Abstracts;
using Library.Model.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // Dependency Injection (Garsona, hangi aşçıdan yemek isteyeceğini söylüyoruz)
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        // 1. İsteği al ve doğrudan Business katmanına (AuthManager) gönder
        var result = await _authService.RegisterAsync(registerDto);

        // 2. Kutu (Result) başarılıysa HTTP 200 (OK) dön
        if (result.Success)
        {
            return Ok(result);
        }

        // 3. Başarısızsa HTTP 400 (Bad Request) dön ve hata kutusunu ver
        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
 
}