using System.Security.Claims;
using Library.Business.Abstracts;
using Library.Model.Dtos.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Member")] // Sadece kütüphane üyeleri rezervasyon yapabilir
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        // 1. Kimlik Tespiti: Token'dan işlemleri yapan kullanıcının ID'sini (int) söküp alıyoruz
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        // 2. İşlemi Devretme: Sadece kitabın ExternalId'sini taşıyan DTO ve token'dan çıkan UserId servise gidiyor
        var result = await _reservationService.CreateReservationAsync(userId, dto);

        // 3. Yanıt Döndürme
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Librarian")] 
    public async Task<IActionResult> GetAllReservations()
    {
        var result = await _reservationService.GetAllReservationsAsync();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}