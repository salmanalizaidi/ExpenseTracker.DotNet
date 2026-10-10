using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
        => Ok(await _userService.GetUserByIdAsync());

    [HttpPatch("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile(UserProfileRequestDto request)
        => Ok(await _userService.UpdateUserProfileAsync(request));

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        await _userService.ChangePasswordAsync(dto);
        return NoContent();
    }

    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        await _userService.DeleteUserByIdAsync();
        return NoContent();
    }
}
