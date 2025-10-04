// Controllers/BusController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;              // MVC types
using Microsoft.EntityFrameworkCore;         // EF Core

using BusReservation.API.Data;               // ApplicationDbContext
using BusReservation.API.Models;             // Bus entity
using BusReservation.API.Dtos;               // BusDto (if you have it)

namespace BusReservation.API.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")] // fully-qualify to avoid Route clash
    public class BusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BusController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bus>>> GetBuses()
        {
            var list = await _context.Buses.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bus>> GetBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null) return NotFound();
            return bus;
        }

        [HttpPost]
        public async Task<ActionResult<Bus>> CreateBus([FromBody] BusDto dto)
        {
            var bus = new Bus
            {
                BusNumber = dto.BusNumber,
                BusType = dto.BusType,
                Capacity = dto.Capacity,
                PlateNumber = dto.PlateNumber
            };

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBus), new { id = bus.Id }, bus);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] BusDto dto)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null) return NotFound();

            bus.BusNumber = dto.BusNumber;
            bus.BusType = dto.BusType;
            bus.Capacity = dto.Capacity;
            bus.PlateNumber = dto.PlateNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null) return NotFound();

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
