using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(e => e.UserId == userId).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByCategoryIdAsync(Guid categoryId) =>
        await _dbSet.Where(e => e.CategoryId == categoryId).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime from, DateTime to) =>
        await _dbSet
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Date >= from && t.Date <= to)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

    public async Task<decimal> GetTotalByUserAndTypeAsync(Guid userId, TransactionType type, DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.Where(t => t.UserId == userId && t.Type == type);
        if (from.HasValue) query = query.Where(t => t.Date >= from.Value);
        if (to.HasValue) query = query.Where(t => t.Date <= to.Value);
        return await query.SumAsync(t => t.Amount);
    }

    public async Task<IEnumerable<Transaction>> GetRecentByUserAsync(Guid userId, int limit) =>
        await _dbSet
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(limit)
            .ToListAsync();
}
