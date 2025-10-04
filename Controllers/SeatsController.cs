using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservation.API.Data;
using BusReservation.API.Models;

namespace BusReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SeatsController(ApplicationDbContext db) { _db = db; }

        // Returns full seat list for the bus in this schedule + which are taken
        [HttpGet("by-schedule/{scheduleId}")]
        public async Task<IActionResult> GetBySchedule(int scheduleId)
        {
            var schedule = await _db.Schedules
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
            if (schedule == null) return NotFound("Schedule not found.");

            var allSeats = await _db.BusSeats
                .Where(bs => bs.BusId == schedule.BusId)
                .OrderBy(bs => bs.SeatNumber)
                .ToListAsync();

            var taken = await _db.Reservations
                .Where(r => r.ScheduleId == scheduleId)
                .Select(r => r.SeatNumber)
                .ToListAsync();

            var result = allSeats.Select(s => new
            {
                seatId = s.Id,
                seatNumber = s.SeatNumber,
                isReserved = taken.Contains(s.SeatNumber)
            });

            return Ok(result);
        }

        // Helper: generate seats for a bus like 2-2 layout with N rows: 1A,1B | 1C,1D...
        // Example body: { "rows": 10, "leftCols": 2, "rightCols": 2 }
        public class GenerateSeatsRequest
        {
            public int Rows { get; set; } = 10;
            public int LeftCols { get; set; } = 2;
            public int RightCols { get; set; } = 2;
            public bool UseLetters { get; set; } = true; // A,B,C,D across row
        }

        [HttpPost("generate/{busId}")]
        public async Task<IActionResult> Generate(int busId, [FromBody] GenerateSeatsRequest req)
        {
            var bus = await _db.Buses.FindAsync(busId);
            if (bus == null) return NotFound("Bus not found.");

            // Avoid duplicates
            var existing = await _db.BusSeats.Where(x => x.BusId == busId).AnyAsync();
            if (existing) return Conflict("Seats already exist for this bus.");

            var seats = new List<BusSeat>();
            for (int row = 1; row <= req.Rows; row++)
            {
                var letters = new List<string>();
                int totalCols = req.LeftCols + req.RightCols;
                for (int i = 0; i < totalCols; i++)
                {
                    letters.Add(req.UseLetters ? ((char)('A' + i)).ToString() : (i + 1).ToString());
                }
                foreach (var letter in letters)
                {
                    seats.Add(new BusSeat
                    {
                        BusId = busId,
                        SeatNumber = $"{row}{letter}"
                    });
                }
            }

            _db.BusSeats.AddRange(seats);
            await _db.SaveChangesAsync();
            return Ok(new { created = seats.Count });
        }
    }
}
