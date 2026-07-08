namespace ExpenseTracker.Application.DTOs;

public class CreateExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}