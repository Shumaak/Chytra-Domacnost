using Chytra_domacnost.Enums;

namespace Chytra_domacnost.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime LastLogin { get; set; }
}
