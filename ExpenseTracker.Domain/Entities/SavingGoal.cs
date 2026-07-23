using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class SavingGoal: BaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime Deadline { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}