using Microsoft.AspNetCore.Identity;
using BeautyPoint.Models;

public class UserService
{
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public string HashPassword(string password)
    {
        var user = new User();
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var user = new User();
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }

    public User CreateUser(string username, string password)
    {
        var user = new User { UserName = username };
        user.PasswordHash = HashPassword(password);
        return user;
    }
}
