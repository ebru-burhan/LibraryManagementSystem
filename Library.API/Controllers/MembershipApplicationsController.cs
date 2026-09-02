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
    public async Task<IActionResult> Apply(
        [FromForm] CreateMembershipApplicationDto createMembershipApplicationDto,
        IFormFile? pictureFile,
        IFormFile? documentFile)
    {
        // 1. KİMLİK TESPİTİ: JWT Token'dan Kullanıcı ID'sini yakalıyoruz
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        if (pictureFile != null && pictureFile.Length > 0)
        {
            createMembershipApplicationDto.PictureUrl = await SaveUploadAsync(pictureFile, "wwwroot/uploads/members");
        }

        if (documentFile != null && documentFile.Length > 0)
        {
            createMembershipApplicationDto.DocumentUrl = await SaveUploadAsync(documentFile, "wwwroot/uploads/documents");
        }

        // 3. İŞLEMİ DEVRETME: Token'dan aldığımız 'userId'yi ve güncellenen DTO'yu Business'a gönderiyoruz
        var result = await _membershipApplicationService.CreateApplicationAsync(userId, createMembershipApplicationDto);

        // 4. STANDART YANIT
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }


    [Authorize]
    [HttpGet("my-application")]
    public async Task<IActionResult> GetMyApplication()
    {
        // 1. Yine token'dan gizlice ID'yi alıyoruz (Senin Apply metodundaki aynı harika mantık)
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { Message = "Güvenlik ihlali: Geçersiz kullanıcı kimliği." });
        }

        // 2. Bu sefer yazma (Create) değil, okuma (Get) yapıyoruz.
        // (Bunun için IMembershipApplicationService içinde GetByUserIdAsync metodunun olması gerekir)
        var result = await _membershipApplicationService.GetByUserIdAsync(userId);

        if (result.Success)
        {
            return Ok(result); // Başvuru varsa HTTP 200 döner (Frontend bunu yakalar ve bilgi kartını çizer)
        }

        // Başvuru yoksa hata döner (Frontend catch bloğuna düşer ve formu çizer)
        return BadRequest(result);
    }

    [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip JWT token'lar buraya girebilir
    [HttpGet("all-applications")]
    public async Task<IActionResult> GetAllApplications()
    {
        // 1. İŞLEMİ DEVRETME: Doğrudan servisteki metodumuzu çağırıyoruz.
        var result = await _membershipApplicationService.GetAllApplicationsDetailsAsync();

        // 2. STANDART YANIT: Liste doluysa HTTP 200 (OK), boş veya hatalıysa HTTP 400 (BadRequest) dönüyoruz.
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }



    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveApplication([FromRoute] int id)
    {
        // 1. İŞLEMİ DEVRETME: Route'dan gelen 'id'yi (applicationId) servise gönderiyoruz
        var result = await _membershipApplicationService.ApproveApplicationAsync(id);

        // 2. STANDART YANIT: Onaylama, üye oluşturma ve rol atama işlemleri tamamen başarılıysa HTTP 200
        if (result.Success)
        {
            return Ok(result);
        }

        // İş kurallarına takılırsa (zaten onaylı, başvuru yok vs.) HTTP 400
        return BadRequest(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectApplication([FromRoute] int id)
    {
        var result = await _membershipApplicationService.RejectApplicationAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    private static async Task<string> SaveUploadAsync(IFormFile file, string relativeFolder)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString() + extension;
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var publicFolder = relativeFolder.Replace("wwwroot", string.Empty).Replace("\\", "/").Trim('/');
        return $"/{publicFolder}/{fileName}";
    }
}