using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(AppDbContext context) : base(context) { }

    public async Task<Budget?> GetCurrentMonthByUserAsync(Guid userId, int month, int year) =>
        await _dbSet.FirstOrDefaultAsync(b => b.UserId == userId && b.Month == month && b.Year == year);
}
