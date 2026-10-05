using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync();
    Task<UserDto> UpdateUserProfileAsync(UserProfileRequestDto profile);
    Task ChangePasswordAsync(ChangePasswordDto dto);
    Task<bool> DeleteUserByIdAsync();
}
