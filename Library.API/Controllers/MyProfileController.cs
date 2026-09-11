using Library.Business.Abstracts;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Member")] // Zırh: Sınıftaki tüm uçlara sadece 'Member' rolü girebilir
public class MyProfileController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly IReservationService _reservationService;
    private readonly ILoanService _loanService;
    private readonly IPenaltyService _penaltyService;

    public MyProfileController(IMemberService memberService, IReservationService reservationService, ILoanService loanService, IPenaltyService penaltyService)
    {
        _memberService = memberService;
        _reservationService = reservationService;
        _loanService = loanService;
        _penaltyService = penaltyService;
    }



    [HttpGet("my-loans")]
    public async Task<IActionResult> GetMyLoans()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        var result = await _loanService.GetLoansByUserIdAsync(userId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-penalties")]
    public async Task<IActionResult> GetMyPenalties()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        var result = await _penaltyService.GetPenaltiesByUserIdAsync(userId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-reservations")]
    public async Task<IActionResult> GetMyReservations()
    {
        // 1. Token'ın içine mühürlediğimiz User.Id (int) değerini okuyoruz
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        // 2. Servise token'dan çıkan saf 'int userId'yi gönderiyoruz
        var result = await _reservationService.GetReservationsByUserIdAsync(userId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-details")]
    public async Task<IActionResult> GetMyDetails()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        var result = await _memberService.GetMyProfileByUserIdAsync(userId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

}