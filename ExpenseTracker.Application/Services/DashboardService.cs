using ExpenseTracker.Application.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public class DashboardService : BaseService, IDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ISavingsGoalRepository _savingsGoalRepository;

    public DashboardService(
        ITransactionRepository transactionRepository,
        IBudgetRepository budgetRepository,
        ISavingsGoalRepository savingsGoalRepository,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _savingsGoalRepository = savingsGoalRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var userId = CurrentUserId;
        var now = DateTime.UtcNow;

        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var thisMonthEnd = thisMonthStart.AddMonths(1).AddTicks(-1);

        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var lastMonthEnd = thisMonthStart.AddTicks(-1);

        // Sequential awaits — EF Core DbContext is not thread-safe and cannot
        // handle concurrent queries on the same instance (Task.WhenAll would cause
        // "A second operation was started on this context" InvalidOperationException).
        var totalIncome        = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Income);
        var totalExpense       = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Expense);
        var thisMonthIncome    = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Income,  thisMonthStart, thisMonthEnd);
        var thisMonthExpense   = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Expense, thisMonthStart, thisMonthEnd);
        var lastMonthIncome    = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Income,  lastMonthStart, lastMonthEnd);
        var lastMonthExpense   = await _transactionRepository.GetTotalByUserAndTypeAsync(userId, TransactionType.Expense, lastMonthStart, lastMonthEnd);
        var thisMonthTxns      = await _transactionRepository.GetByUserAndDateRangeAsync(userId, thisMonthStart, thisMonthEnd);
        var recentTxns         = await _transactionRepository.GetRecentByUserAsync(userId, 5);
        var budget             = await _budgetRepository.GetCurrentMonthByUserAsync(userId, now.Month, now.Year);
        var savingsGoal        = await _savingsGoalRepository.GetActiveByUserAsync(userId);

        var totalBalance     = totalIncome - totalExpense;
        var thisMonthSavings = thisMonthIncome - thisMonthExpense;
        var lastMonthSavings = lastMonthIncome - lastMonthExpense;

        var balance = new BalanceSummaryDto(totalBalance, thisMonthSavings, lastMonthSavings);

        // Monthly spending
        var monthlyLimit = budget?.MonthlyLimit ?? 0m;
        var spent        = thisMonthExpense;
        var remaining    = monthlyLimit - spent;
        var daysLeft     = (thisMonthEnd.Date - now.Date).Days;
        var monthlySpending = new MonthlySpendingDto(spent, monthlyLimit, remaining, daysLeft);

        // Savings goal
        SavingsGoalSummaryDto? savingsGoalDto = null;
        if (savingsGoal is { } sg)
        {
            var progress = sg.TargetAmount > 0
                ? Math.Round(sg.CurrentAmount / sg.TargetAmount * 100, 2)
                : 0m;
            savingsGoalDto = new SavingsGoalSummaryDto(sg.Id, sg.Name, sg.CurrentAmount, sg.TargetAmount, progress, sg.Deadline);
        }

        // Category distribution — expenses only this month
        var thisMonthTransactions = thisMonthTxns.ToList();
        var expenseTransactions = thisMonthTransactions.Where(t => t.Type == TransactionType.Expense).ToList();
        var totalExpenseThisMonth = expenseTransactions.Sum(t => t.Amount);

        var categoryDistribution = expenseTransactions
            .GroupBy(t => t.Category)
            .Select(g =>
            {
                var amount = g.Sum(t => t.Amount);
                var percent = totalExpenseThisMonth > 0
                    ? Math.Round(amount / totalExpenseThisMonth * 100, 2)
                    : 0m;
                return new CategoryDistributionDto(
                    g.Key.Id,
                    g.Key.Name,
                    g.Key.Icon,
                    g.Key.Color,
                    amount,
                    percent);
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        // Recent transactions
        var recentTransactions = MapToRecentDtos(recentTxns);

        return new DashboardDto(balance, monthlySpending, savingsGoalDto, categoryDistribution, recentTransactions);
    }

    public async Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int limit)
    {
        var transactions = await _transactionRepository.GetRecentByUserAsync(CurrentUserId, limit);
        return MapToRecentDtos(transactions);
    }

    private static IEnumerable<RecentTransactionDto> MapToRecentDtos(IEnumerable<Transaction> transactions) =>
        transactions.Select(t => new RecentTransactionDto(
            t.Id,
            t.Title,
            t.Amount,
            t.Type,
            t.Category.Name,
            t.Category.Icon,
            t.Category.Color,
            t.Date));
}
