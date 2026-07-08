using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto);
    Task DeleteAsync(Guid id);
}