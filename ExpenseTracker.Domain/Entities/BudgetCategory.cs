using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class BudgetCategory: BaseEntity
{
    public Guid BudgetId { get; set; }
    public Budget Budget { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal SpendingLimit { get; set; }
}