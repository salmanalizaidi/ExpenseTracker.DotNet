using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class ScheduledPayment: BaseEntity
{
    [MaxLength(200)] 
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}