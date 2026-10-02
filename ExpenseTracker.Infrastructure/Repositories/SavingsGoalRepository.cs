using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class SavingsGoalRepository : BaseRepository<SavingsGoal>, ISavingsGoalRepository
{
    public SavingsGoalRepository(AppDbContext context) : base(context) { }

    public async Task<SavingsGoal?> GetActiveByUserAsync(Guid userId) =>
        await _dbSet
            .Where(sg => sg.UserId == userId && sg.CurrentAmount < sg.TargetAmount)
            .OrderBy(sg => sg.Deadline)
            .FirstOrDefaultAsync();
}
