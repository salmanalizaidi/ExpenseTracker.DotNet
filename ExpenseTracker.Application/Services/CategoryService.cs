using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using Mapster;

namespace ExpenseTracker.Application.Services;

public class CategoryService: ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId) =>
        (await _categoryRepository.GetByUserIdAsync(userId)).Adapt<IEnumerable<CategoryDto>>();

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        var category = dto.Adapt<Category>();
        category.UserId = userId;
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Adapt<CategoryDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _categoryRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
