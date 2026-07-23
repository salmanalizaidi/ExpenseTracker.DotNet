using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Budget:  BaseEntity
{
    public Decimal MonthlyLimit { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}