using Library.Business.Abstracts;
using Library.Model.Dtos.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Librarian")]
public class LostBooksController : ControllerBase
{
    private readonly ILostBookService _lostBookService;

    public LostBooksController(ILostBookService lostBookService)
    {
        _lostBookService = lostBookService;
    }

    [HttpPost("report")]
    public async Task<IActionResult> ReportLostBook([FromBody] ReportLostBookDto dto)
    {
        var result = await _lostBookService.ReportLostBookAsync(dto);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis()
    {
        var result = await _lostBookService.GetLostBookKpisAsync();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _lostBookService.GetAllLostBooksAsync();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}