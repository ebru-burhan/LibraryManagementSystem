using Library.Business.Abstracts;
using Library.Model.Dtos.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleDto createRoleDto)
    {
        var result = await _roleService.AddAsync(createRoleDto);

        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}