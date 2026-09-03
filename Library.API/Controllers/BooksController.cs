using Library.Business.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            // Not: İleride IBookService ve BookManager güncellendiğinde burası:
            // await _bookService.GetAllBooksAsync(search); şeklinde parametre alacak.
            var result = await _bookService.GetAllBooksAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }





    }
}
