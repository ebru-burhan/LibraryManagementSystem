using Library.Business.Abstracts;
using Library.Model.Dtos.Catalog;
using Microsoft.AspNetCore.Authorization;
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
            // Not: İleride search parametresi manager'a eklendiğinde buraya gönderebilirsin.
            var result = await _bookService.GetAllAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // Dikkat: Route içinde {id:guid} kısıtlaması kullandık!
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            // Eğer kitap bulunamazsa 404 dönmek API standartlarına daha uygundur
            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateBookDto dto)
        {
            var result = await _bookService.AddAsync(dto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _bookService.DeleteAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}