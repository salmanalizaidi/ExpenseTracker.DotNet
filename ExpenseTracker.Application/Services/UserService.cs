using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Repositories;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class UserService : BaseService, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UserProfileRequestDto> _profileValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

    public UserService(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<UserProfileRequestDto> profileValidator,
        IValidator<ChangePasswordDto> changePasswordValidator) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _profileValidator = profileValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public async Task<UserDto> GetUserByIdAsync()
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId)
                   ?? throw new KeyNotFoundException("User not found");
        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> UpdateUserProfileAsync(UserProfileRequestDto profile)
    {
        var validation = await _profileValidator.ValidateAsync(profile);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Errors.First().ErrorMessage);

        var user = await _userRepository.GetByIdAsync(CurrentUserId)
                   ?? throw new KeyNotFoundException("User not found");

        // Uniqueness check only when the username actually changed
        if (user.Username != profile.Username &&
            await _userRepository.UsernameExistsAsync(profile.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;
        user.Username = profile.Username;

        // null = no change; "" = remove; non-empty = set new avatar
        if (profile.AvatarBase64 is not null)
        {
            user.AvatarBase64 = profile.AvatarBase64 == string.Empty ? null : profile.AvatarBase64;
        }

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user.Adapt<UserDto>();
    }

    public async Task ChangePasswordAsync(ChangePasswordDto dto)
    {
        var validation = await _changePasswordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Errors.First().ErrorMessage);

        var user = await _userRepository.GetByIdAsync(CurrentUserId)
                   ?? throw new KeyNotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserByIdAsync()
    {
        await _userRepository.DeleteAsync(CurrentUserId);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
