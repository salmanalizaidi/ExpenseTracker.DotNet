using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Repositories;

public interface ITransactionRepository: IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime from, DateTime to);
    Task<decimal> GetTotalByUserAndTypeAsync(Guid userId, TransactionType type, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<Transaction>> GetRecentByUserAsync(Guid userId, int limit);
}