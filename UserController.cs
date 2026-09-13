using System.Security.Claims;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserStore users) : ControllerBase
{
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok(new { message = "Доступ разрешен", userId, username, role });
    }

    [HttpGet("user-data")]
    [Authorize(Roles = "User")]
    public IActionResult GetUserData()
    {
        return Ok(new { message = "Данные доступны пользователю с ролью User" });
    }

    [HttpGet("admin-data")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdminData()
    {
        return Ok(new { message = "Секретные данные для админа" });
    }

    [HttpGet("moderator-data")]
    [Authorize(Policy = "AdminOrModerator")]
    public IActionResult GetModeratorData()
    {
        return Ok(new { message = "Доступ для админа или модератора" });
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteUser(int id)
    {
        return users.Delete(id)
            ? Ok(new { message = $"Пользователь {id} удалён" })
            : NotFound(new { message = "Пользователь не найден" });
    }
}
