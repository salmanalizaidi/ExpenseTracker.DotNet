using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class CategoryService : BaseService, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryDto> _createValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IValidator<CreateCategoryDto> createValidator) : base(httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
        (await _categoryRepository.GetByUserIdAsync(CurrentUserId)).Adapt<IEnumerable<CategoryDto>>();

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var category = dto.Adapt<Category>();
        category.UserId = CurrentUserId;
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Adapt<CategoryDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Category {id} not found");

        // Ownership check — prevent users from deleting other users' categories
        if (category.UserId != CurrentUserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this category.");

        await _categoryRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
