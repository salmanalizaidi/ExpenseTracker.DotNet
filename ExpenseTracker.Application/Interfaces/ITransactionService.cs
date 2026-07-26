using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllAsync();
    Task<TransactionDto?> GetByIdAsync(Guid id);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
    Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task DeleteAsync(Guid id);
}