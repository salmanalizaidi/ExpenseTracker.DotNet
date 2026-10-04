using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Repositories;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<Budget?> GetCurrentMonthByUserAsync(Guid userId, int month, int year);
}
