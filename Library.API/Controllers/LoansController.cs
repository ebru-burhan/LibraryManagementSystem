using Library.Business.Abstracts;
using Library.Model.Dtos.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Librarian")] // Sadece yetkili personel ödünç verebilir
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost("borrow")]
    public async Task<IActionResult> BorrowBook([FromBody] CreateLoanDto dto)
    {
        var result = await _loanService.CreateLoanAsync(dto);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}