// Controllers/ScheduleController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservation.API.Data;
using BusReservation.API.Models;
using BusReservation.API.Dtos;

namespace BusReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ScheduleController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            return await _context.Schedules
                .Include(s => s.BusRoute)   // was: .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver)     // remove if you don't have Driver
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.BusRoute)   // was: .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null) return NotFound();
            return schedule;
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> CreateSchedule([FromBody] ScheduleDto dto)
        {
            if (dto.ArrivalTime <= dto.DepartureTime)
                return BadRequest("ArrivalTime must be after DepartureTime.");

            var schedule = new Schedule
            {
                BusRouteId = dto.BusRouteId,  // was: RouteId
                BusId = dto.BusId,
                DriverId = dto.DriverId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();

            if (dto.ArrivalTime <= dto.DepartureTime)
                return BadRequest("ArrivalTime must be after DepartureTime.");

            schedule.BusRouteId = dto.BusRouteId; // was: RouteId
            schedule.BusId = dto.BusId;
            schedule.DriverId = dto.DriverId;
            schedule.DepartureTime = dto.DepartureTime;
            schedule.ArrivalTime = dto.ArrivalTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
