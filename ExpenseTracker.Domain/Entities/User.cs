using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class User: BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime LastLoginAt { get; set; }

    /// <summary>
    /// Profile picture stored as a Base64-encoded data URI.
    /// Null means no avatar set. Empty string means avatar was removed.
    /// </summary>
    public string? AvatarBase64 { get; set; }

    /// <summary>
    /// Subscription plan tier. "Free" by default.
    /// Future: replace with a Plan entity FK.
    /// </summary>
    public string PlanTier { get; set; } = "Free";

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}