using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> UpdateUserProfileAsync(UserProfileRequestDto profile);
}