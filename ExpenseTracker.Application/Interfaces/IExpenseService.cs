using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAllAsync(Guid userId);
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<ExpenseDto> CreateAsync(Guid userId, CreateExpenseDto dto);
    Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseDto dto);
    Task DeleteAsync(Guid id);
}