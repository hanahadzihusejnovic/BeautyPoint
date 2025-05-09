using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    [Authorize]
    [HttpGet("GetDashboard")]
    public IActionResult GetDashboard()
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == "UserRole")?.Value;

        if (userRole == "Admin")
        {
            return Ok("Welcome to Admin Dashboard");
        }
        else if (userRole == "Employee")
        {
            return Ok("Welcome to Employee Dashboard");
        }
        else
        {
            return Ok("Welcome to Client Dashboard");
        }
    }
}
