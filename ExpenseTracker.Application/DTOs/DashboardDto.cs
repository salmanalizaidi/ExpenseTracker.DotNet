using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.DTOs;

public record DashboardDto(
    BalanceSummaryDto Balance,
    MonthlySpendingDto MonthlySpending,
    SavingsGoalSummaryDto? SavingsGoal,
    IEnumerable<CategoryDistributionDto> CategoryDistribution,
    IEnumerable<RecentTransactionDto> RecentTransactions
);

public record BalanceSummaryDto(
    decimal TotalBalance,
    decimal MonthlySavings,
    decimal LastMonthSavings
);

public record MonthlySpendingDto(
    decimal Spent,
    decimal Limit,
    decimal Remaining,
    int DaysLeft
);

public record SavingsGoalSummaryDto(
    Guid Id,
    string Name,
    decimal CurrentAmount,
    decimal TargetAmount,
    decimal ProgressPercent,
    DateTime Deadline
);

public record CategoryDistributionDto(
    Guid CategoryId,
    string CategoryName,
    string? Icon,
    string? Color,
    decimal Amount,
    decimal Percent
);

public record RecentTransactionDto(
    Guid Id,
    string Title,
    decimal Amount,
    TransactionType Type,
    string CategoryName,
    string? CategoryIcon,
    string? CategoryColor,
    DateTime Date
);
