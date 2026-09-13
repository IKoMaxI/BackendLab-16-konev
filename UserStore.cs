using JwtAuth.Models;

namespace JwtAuth.Services;

public class UserStore
{
    private readonly object _syncRoot = new();
    private readonly List<User> _users =
    [
        CreateUser(1, "admin", "admin123", "Admin"),
        CreateUser(2, "user", "user123", "User"),
        CreateUser(3, "moderator", "moderator123", "Moderator")
    ];

    public User? FindByUsername(string username)
    {
        lock (_syncRoot)
        {
            return _users.FirstOrDefault(user =>
                user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }

    public bool Exists(string username)
    {
        lock (_syncRoot)
        {
            return _users.Any(user =>
                user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }

    public User? Add(string username, string password, string role)
    {
        lock (_syncRoot)
        {
            if (_users.Any(item => item.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return null;
            var user = CreateUser(
                _users.Count == 0 ? 1 : _users.Max(item => item.Id) + 1,
                username,
                password,
                role);
            _users.Add(user);
            return user;
        }
    }

    public bool Delete(int id)
    {
        lock (_syncRoot)
        {
            var user = _users.FirstOrDefault(item => item.Id == id);
            return user is not null && _users.Remove(user);
        }
    }

    private static User CreateUser(int id, string username, string password, string role)
    {
        return new User
        {
            Id = id,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
    }
}
