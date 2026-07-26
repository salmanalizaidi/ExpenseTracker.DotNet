using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.DTOs;

public class CreateTransactionDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}