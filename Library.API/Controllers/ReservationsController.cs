using System.Security.Claims;
using Library.Business.Abstracts;
using Library.Model.Dtos.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Member")] // Sadece üyeler rezervasyon yapabilir
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        var result = await _reservationService.CreateReservationAsync(userId, dto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Librarian")] // Sadece kütüphane personeli tümünü görebilir
    public async Task<IActionResult> GetAllReservations()
    {
        var result = await _reservationService.GetAllReservationsAsync();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }


    // MemberDetailAdminPage için

    [HttpGet("member/{memberExternalId}")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetReservationsByMember([FromRoute] Guid memberExternalId)
    {
        // Admin, URL üzerinden incelemek istediği üyenin ExternalId'sini gönderir
        var result = await _reservationService.GetReservationsByMemberIdAsync(memberExternalId);

        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}