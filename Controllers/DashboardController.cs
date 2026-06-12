using Hostelfoodsystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hostelfoodsystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalMeals = await _context.MealRecords.CountAsync();
            var totalPrepared = await _context.MealRecords.SumAsync(x => (int?)x.PreparedCount) ?? 0;
            var totalConsumed = await _context.MealRecords.SumAsync(x => (int?)x.ConsumedCount) ?? 0;
            var totalWaste = await _context.MealRecords.SumAsync(x => (int?)x.WasteCount) ?? 0;

            return Ok(new
            {
                TotalStudents = totalStudents,
                TotalMeals = totalMeals,
                TotalPrepared = totalPrepared,
                TotalConsumed = totalConsumed,
                TotalWaste = totalWaste
            });
        }
    }
}