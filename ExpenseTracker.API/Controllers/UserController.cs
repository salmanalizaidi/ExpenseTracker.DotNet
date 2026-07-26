using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPatch("profile")]
    public async Task<UserDto> UpdateProfile(UserProfileRequestDto request)
    {
        var response = await _userService.UpdateUserProfileAsync(request);
        return response;
    }
}