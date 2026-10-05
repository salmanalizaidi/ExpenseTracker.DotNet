namespace ExpenseTracker.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarBase64 { get; set; }
    public string PlanTier { get; set; } = "Free";
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}
