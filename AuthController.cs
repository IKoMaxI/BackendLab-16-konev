using JwtAuth.Models;
using JwtAuth.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserStore users, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = users.FindByUsername(model.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        var token = tokenService.GenerateToken(user);
        return Ok(new
        {
            token,
            userId = user.Id,
            username = user.Username,
            role = user.Role,
            expiresIn = 60
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        if (users.Exists(model.Username))
        {
            return Conflict(new { message = "Пользователь уже существует" });
        }

        if (!model.Role.Equals("User", StringComparison.OrdinalIgnoreCase))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Самостоятельная регистрация разрешена только с ролью User" });

        var role = "User";
        var user = users.Add(model.Username, model.Password, role);
        if (user is null)
            return Conflict(new { message = "Пользователь уже существует" });

        return Ok(new
        {
            message = "Пользователь зарегистрирован",
            userId = user.Id,
            username = user.Username,
            role = user.Role
        });
    }
}
