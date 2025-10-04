// Controllers/RouteController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservation.API.Data;
using BusReservation.API.Models;
using BusReservation.API.Dtos; // <-- ensure your DTO namespace is DTOs (not Dtos)

namespace BusReservation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RouteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Route
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusRoute>>> GetRoutes()
        {
            var routes = await _context.BusRoutes.AsNoTracking().ToListAsync(); // <-- BusRoutes
            return Ok(routes);
        }

        // GET: api/Route/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BusRoute>> GetRoute(int id)
        {
            var route = await _context.BusRoutes.FindAsync(id); // <-- BusRoutes
            if (route == null) return NotFound();
            return route;
        }

        // POST: api/Route
        [HttpPost]
        public async Task<ActionResult<BusRoute>> CreateRoute([FromBody] RouteDto dto)
        {
            var route = new BusRoute
            {
                StartLocation = dto.StartLocation,
                EndLocation = dto.EndLocation,
                DistanceInKm = dto.DistanceInKm,   // <-- ensure BusRoute has these props
                EstimatedTime = dto.EstimatedTime
            };

            _context.BusRoutes.Add(route); // <-- BusRoutes
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
        }

        // PUT: api/Route/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteDto dto)
        {
            var route = await _context.BusRoutes.FindAsync(id); // <-- BusRoutes
            if (route == null) return NotFound();

            route.StartLocation = dto.StartLocation;
            route.EndLocation = dto.EndLocation;
            route.DistanceInKm = dto.DistanceInKm;
            route.EstimatedTime = dto.EstimatedTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Route/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.BusRoutes.FindAsync(id); // <-- BusRoutes
            if (route == null) return NotFound();

            _context.BusRoutes.Remove(route); // <-- BusRoutes
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByUser(string userId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.BusRoute)
                .Include(r => r.Schedule.Bus)
                .Include(r => r.Schedule.Driver)
                .Where(r => r.CreatedBy == userId)
                .ToListAsync();

            return reservations;
        }
    }
}
