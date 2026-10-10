namespace ExpenseTracker.Application.DTOs;

public class UserProfileRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// null = do not change avatar; "" = remove avatar; non-empty = set new avatar.
    /// </summary>
    public string? AvatarBase64 { get; set; }
}
