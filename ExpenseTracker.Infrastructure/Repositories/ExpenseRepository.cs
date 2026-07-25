using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class ExpenseRepository : BaseRepository<Transaction>, IExpenseRepository
{
    public ExpenseRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(e => e.UserId == userId).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByCategoryIdAsync(Guid categoryId) =>
        await _dbSet.Where(e => e.CategoryId == categoryId).ToListAsync();
}
