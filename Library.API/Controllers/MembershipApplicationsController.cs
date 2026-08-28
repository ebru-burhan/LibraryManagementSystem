using System.Security.Claims;
using Library.Business.Abstracts;
using Library.Model.Dtos.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// 1. KORUMA KALKANI: Sadece sisteme giriş yapmış (geçerli JWT Token'ı olan) kişiler bu API'ye ulaşabilir.
[Authorize]
public class MembershipApplicationsController : ControllerBase
{
    private readonly IMembershipApplicationService _membershipApplicationService;

    // Dependency Injection: Controller sadece Interface'i bilir, Manager'ın iç yapısıyla ilgilenmez.
    public MembershipApplicationsController(IMembershipApplicationService membershipApplicationService)
    {
        _membershipApplicationService = membershipApplicationService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply([FromBody] CreateMembershipApplicationDto dto)
    {
        // 2. KİMLİK TESPİTİ (SİHİRLİ KISIM): Dışarıdan ID almıyoruz!
        // JWT Token'ın içindeki NameIdentifier (Kullanıcı ID'si) claim'ini yakalıyoruz.
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // Eğer token'da ID yoksa veya bozuksa, 401 Unauthorized (Yetkisiz) dönüyoruz.
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        // 3. İŞLEMİ DEVRETME: Token'dan güvenle kopardığımız 'userId'yi ve formdan gelen 'dto'yu Business'a gönderiyoruz.
        var result = await _membershipApplicationService.CreateApplicationAsync(userId, dto);

        // 4. STANDART YANIT: İşlem başarılıysa HTTP 200 (OK), iş kurallarına (TC çakışması vb.) takıldıysa HTTP 400 dönüyoruz.
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}