// Controllers/DriverController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;              // ControllerBase, ApiController, Http* attributes
using Microsoft.EntityFrameworkCore;         // EF Core

using BusReservation.API.Data;               // ApplicationDbContext
using BusReservation.API.Models;             // Driver entity
using BusReservation.API.Dtos;               // DriverDto

namespace BusReservation.API.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")] // fully-qualify to avoid [Route] clash
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context) => _context = context;

        // GET: api/driver
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            var list = await _context.Set<Driver>().AsNoTracking().ToListAsync();
            return Ok(list);
        }

        // GET: api/driver/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _context.Set<Driver>().FindAsync(id);
            if (driver == null) return NotFound();
            return driver;
        }

        // POST: api/driver
        [HttpPost]
        public async Task<ActionResult<Driver>> CreateDriver([FromBody] DriverDto dto)
        {
            var driver = new Driver
            {
                FullName = dto.FullName,
                LicenseNumber = dto.LicenseNumber,
                PhoneNumber = dto.PhoneNumber,
                IsActive = dto.IsActive
            };

            _context.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver);
        }

        // PUT: api/driver/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] DriverDto dto)
        {
            var driver = await _context.Set<Driver>().FindAsync(id);
            if (driver == null) return NotFound();

            driver.FullName = dto.FullName;
            driver.LicenseNumber = dto.LicenseNumber;
            driver.PhoneNumber = dto.PhoneNumber;
            driver.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/driver/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Set<Driver>().FindAsync(id);
            if (driver == null) return NotFound();

            _context.Remove(driver);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
