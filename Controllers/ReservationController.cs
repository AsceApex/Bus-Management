// Controllers/ReservationController.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;                 // [ApiController], [Route], ControllerBase, ActionResult
using Microsoft.EntityFrameworkCore;            // Include/ThenInclude, ToListAsync
using BusReservation.API.Data;                  // ApplicationDbContext
using BusReservation.API.Models;                // Reservation, Schedule, BusRoute, Bus, Driver
using BusReservation.API.Dtos;                  // ReservationDto (use DTOs if that’s your namespace)
using Microsoft.AspNetCore.SignalR;             // SignalR hub context
using BusReservation.API.Hubs;                  // SeatsHub
using BusReservation.API.Services;    // IEmailSender

namespace BusReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SeatsHub> _hub;
        private readonly IEmailSender _email;

        public ReservationController(ApplicationDbContext context, IHubContext<SeatsHub> hub, IEmailSender email)
        {
            _context = context;
            _hub = hub;
            _email = email;
        }

        // GET: api/Reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            var data = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.BusRoute)   // adjust to your model (BusRoute vs Route)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Bus)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Driver)
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/Reservation/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var res = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.BusRoute)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Bus)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Driver)
                .FirstOrDefaultAsync(r => r.Id == id);

            return res is null ? NotFound() : res;
        }

        // POST: api/Reservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation([FromBody] ReservationDto dto)
        {
            // prevent duplicate seat on same schedule
            var exists = await _context.Reservations.AnyAsync(r =>
         r.ScheduleId == dto.ScheduleId && r.SeatNumber == dto.SeatNumber);
            if (exists) return Conflict("Seat already reserved.");

            var reservation = new Reservation
            {
                ScheduleId = dto.ScheduleId,
                PassengerName = dto.PassengerName,
                PassengerPhone = dto.PassengerPhone,
                PassengerEmail = dto.PassengerEmail,
                SeatNumber = dto.SeatNumber,
                IsConfirmed = false,
                CreatedBy = dto.CreatedBy   // keep only if your model/DTO has this field
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // 🔔 SignalR: notify all viewers of this schedule that a seat became reserved
            await _hub.Clients.Group($"schedule-{reservation.ScheduleId}")
                .SendAsync("seatUpdated", new
                {
                    scheduleId = reservation.ScheduleId,
                    seatNumber = reservation.SeatNumber,
                    isReserved = true
                });

            // send email (if provided)
            if (!string.IsNullOrWhiteSpace(reservation.PassengerEmail))
            {
                var sch = await _context.Schedules
                  .Include(s => s.BusRoute).Include(s => s.Bus)
                  .FirstAsync(s => s.Id == reservation.ScheduleId);

                var html = ReservationTemplates.Created(
                    reservation.PassengerName,
                    reservation.SeatNumber,
                    sch.DepartureTime,
                    sch.BusRoute.StartLocation,
                    sch.BusRoute.EndLocation,
                    sch.Bus.BusNumber);

                await _email.SendAsync(reservation.PassengerEmail, "Reservation Created", html);
            }

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // PUT: api/Reservation/5/confirm
        [HttpPut("{id:int}/confirm")]
        public async Task<IActionResult> ConfirmReservation(int id)
        {
            var res = await _context.Reservations
    .Include(r => r.Schedule).ThenInclude(s => s.BusRoute)   // note: BusRoute (your model)
    .Include(r => r.Schedule).ThenInclude(s => s.Bus)
    .FirstOrDefaultAsync(r => r.Id == id);

            if (res is null) return NotFound();

            res.IsConfirmed = true;
            await _context.SaveChangesAsync();

            // send confirmation email (no change to existing behavior if email is absent)
            if (!string.IsNullOrWhiteSpace(res.PassengerEmail))
            {
                var busNumber = res.Schedule.Bus?.BusNumber ?? "—";

                var html = ReservationTemplates.Confirmed(
                    res.PassengerName,
                    res.SeatNumber,
                    res.Schedule.DepartureTime,
                    res.Schedule.BusRoute.StartLocation,
                    res.Schedule.BusRoute.EndLocation,
                    busNumber
                );

                await _email.SendAsync(res.PassengerEmail, "Reservation Confirmed ✅", html);
            }
            // (Optional) Broadcast a separate event if your UI needs it
            // await _hub.Clients.Group($"schedule-{res.ScheduleId}")
            //     .SendAsync("reservationConfirmed", new { reservationId = res.Id });

            return NoContent();
        }

        // DELETE: api/Reservation/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var res = await _context.Reservations.FindAsync(id);
            if (res is null) return NotFound();

            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();

            // 🔔 SignalR: notify all viewers of this schedule that a seat became free
            await _hub.Clients.Group($"schedule-{res.ScheduleId}")
                .SendAsync("seatUpdated", new
                {
                    scheduleId = res.ScheduleId,
                    seatNumber = res.SeatNumber,
                    isReserved = false
                });

            return NoContent();
        }
    }
}
