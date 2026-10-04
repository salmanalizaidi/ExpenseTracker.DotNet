using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class UserService: BaseService, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,
        IUnitOfWork unitOfWork) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> UpdateUserProfileAsync(UserProfileRequestDto profile)
    {
        if(await _userRepository.UsernameExistsAsync(profile.Username))
            throw new InvalidOperationException("Username already exists");
        
        var user = await _userRepository.GetByIdAsync(CurrentUserId) ?? throw new KeyNotFoundException("User not found");
        
        user.Username = profile.Username;
        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;
        
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return user.Adapt<UserDto>();
    }

    public async Task<User> GetUserByIdAsync()
    {
        return await _userRepository.GetByIdAsync(CurrentUserId) ?? throw new KeyNotFoundException("User not found");
    }

    public async Task<bool> DeleteUserByIdAsync()
    {
        await _userRepository.DeleteAsync(CurrentUserId);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}