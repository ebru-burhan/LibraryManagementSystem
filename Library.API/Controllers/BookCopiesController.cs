using Library.Business.Abstracts;
using Library.Model.Dtos.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyService _bookCopyService;

    public BookCopiesController(IBookCopyService bookCopyService)
    {
        _bookCopyService = bookCopyService;
    }

    [HttpPost("add")]
    // Dosya yüklemesi iptal edildiği için tekrar [FromBody] kullanıyoruz
    public async Task<IActionResult> AddBookCopy([FromBody] CreateBookCopyDto createBookCopyDto)
    {
        var result = await _bookCopyService.CreateBookCopyAsync(createBookCopyDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}