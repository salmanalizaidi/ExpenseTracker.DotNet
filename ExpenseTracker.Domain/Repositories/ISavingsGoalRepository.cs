using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Repositories;

public interface ISavingsGoalRepository : IRepository<SavingsGoal>
{
    Task<SavingsGoal?> GetActiveByUserAsync(Guid userId);
}
