using Library.Business.Abstracts;
using Library.Model.Dtos.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? search)
    {
        var result = await _memberService.GetAllAsync(status, search);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _memberService.GetByIdAsync(id);
        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateMemberStatusDto dto)
    {
        var result = await _memberService.UpdateStatusAsync(id, dto.StatusCode);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _memberService.DeleteAsync(id);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}