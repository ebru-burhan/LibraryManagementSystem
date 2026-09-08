using Library.Business.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class MyProfileController : ControllerBase
    {
        private readonly IMemberService _memberService;
    public MyProfileController(IMemberService memberService)
    {
        _memberService = memberService;
    }


    [HttpGet("my-penalties")]
    [Authorize(Roles = "Member")] 
    // TODO: Üyenin kendi cezalarını göreceği uç. URL'den ID alınmayacak, JWT token içindeki (ClaimTypes.NameIdentifier) üye ID'si ile _penaltyService üzerinden filtrelenip dönecek!
    public async Task<IActionResult> GetMyPenalties()
    {
        // Token'dan üye ID'si okunup buraya entegre edilecek!!!!!
        throw new NotImplementedException();
    }



}

