using Library.Business.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class PenaltiesController : ControllerBase
{
    private readonly IPenaltyService _penaltyService;

    public PenaltiesController(IPenaltyService penaltyService)
    {
        _penaltyService = penaltyService;
    }

    [HttpGet("member/{memberExternalId}")]
    public async Task<IActionResult> GetPenaltiesByMember(Guid memberExternalId)
    {
        var result = await _penaltyService.GetPenaltiesByMemberIdAsync(memberExternalId);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut("pay/{penaltyExternalId}")]
    public async Task<IActionResult> PayPenalty(Guid penaltyExternalId)
    {
        var result = await _penaltyService.PayPenaltyAsync(penaltyExternalId);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}