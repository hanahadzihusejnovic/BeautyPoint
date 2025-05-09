using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using BeautyPoint.Data; 
using BeautyPoint.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly DatabaseContext _context; 

    public AdminDashboardController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var totalReservations = await _context.Reservations.CountAsync(cancellationToken);
        var totalClients = await _context.Users
            .Where(u => u.Role == UserRole.Client)
            .CountAsync(cancellationToken);
        var totalRevenue = await _context.Payments.SumAsync(p => p.Amount);
        var totalTreatments = await _context.Treatments.CountAsync(cancellationToken); 

        var stats = new
        {
            TotalProducts = totalProducts,
            TotalReservations = totalReservations,
            TotalClients = totalClients,
            TotalRevenue = totalRevenue,
            TotalTreatments = totalTreatments 
        };

        return Ok(stats);
    }

}
