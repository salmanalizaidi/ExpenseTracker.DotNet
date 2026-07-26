namespace ExpenseTracker.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}