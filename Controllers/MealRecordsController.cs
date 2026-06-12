using Hostelfoodsystem.Data;
using Hostelfoodsystem.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hostelfoodsystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MealRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealRecord>>> GetMealRecords()
        {
            return await _context.MealRecords.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MealRecord>> GetMealRecord(int id)
        {
            var meal = await _context.MealRecords.FindAsync(id);

            if (meal == null)
                return NotFound("Meal record not found");

            return meal;
        }

        [HttpPost]
        public async Task<ActionResult<MealRecord>> AddMealRecord([FromBody] MealRecord meal)
        {
            meal.Id = 0;

            meal.WasteCount = meal.PreparedCount - meal.ConsumedCount;

            if (meal.WasteCount < 0)
                return BadRequest("Consumed count cannot be greater than prepared count");

            _context.MealRecords.Add(meal);
            await _context.SaveChangesAsync();

            return Ok(meal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealRecord(int id, [FromBody] MealRecord meal)
        {
            if (id != meal.Id)
                return BadRequest("Meal record ID mismatch");

            meal.WasteCount = meal.PreparedCount - meal.ConsumedCount;

            if (meal.WasteCount < 0)
                return BadRequest("Consumed count cannot be greater than prepared count");

            _context.Entry(meal).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(meal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealRecord(int id)
        {
            var meal = await _context.MealRecords.FindAsync(id);

            if (meal == null)
                return NotFound("Meal record not found");

            _context.MealRecords.Remove(meal);
            await _context.SaveChangesAsync();

            return Ok("Meal record deleted successfully");
        }
    }
}