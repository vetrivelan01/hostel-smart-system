using Hostelfoodsystem.Data;
using Hostelfoodsystem.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hostelfoodsystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance()
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendanceById(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
                return NotFound("Attendance record not found");

            return attendance;
        }

        [HttpPost]
        public async Task<ActionResult<Attendance>> AddAttendance([FromBody] Attendance attendance)
        {
            attendance.Id =0;

            var studentExists = await _context.Students.AnyAsync(s => s.Id == attendance.StudentId);

            if (!studentExists)
                return BadRequest("Invalid Student ID");

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(attendance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] Attendance attendance)
        {
            if (id != attendance.Id)
                return BadRequest("Attendance record ID mismatch");

            var existing = await _context.Attendances.FindAsync(id);
            if (existing == null)
                return NotFound("Attendance record not found");

            var studentExists = await _context.Students.AnyAsync(s => s.Id == attendance.StudentId);
            if (!studentExists)
                return BadRequest("Invalid Student ID");

            existing.IsPresent = attendance.IsPresent;
            existing.Date = attendance.Date;
            existing.StudentId = attendance.StudentId;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult> GetAttendanceByDate(DateTime date)
        {
            var count = await _context.Attendances
                .Where(a => a.Date.Date == date.Date && a.IsPresent)
                .CountAsync();

            return Ok(new
            {
                Date = date.ToString("yyyy-MM-dd"),
                PresentCount = count
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return NotFound("Attendance record not found");

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok("Attendance deleted successfully");
        }
    }
}