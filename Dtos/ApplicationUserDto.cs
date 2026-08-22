using Task4.Models;

namespace Task4.Dtos;

public record ApplicationUserDto(string? UserName, string? Email, Status Status, DateTime LastLogin, string UserId)
{
    public string LastActivity { get; init; } = string.Empty;
}
