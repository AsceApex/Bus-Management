using System.Globalization;
using System.Text;
using BusReservation.API.Data;
using BusReservation.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BusReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ReportsController(ApplicationDbContext db) { _db = db; }

        // Shared query builder - Changed from dynamic to a strongly-typed anonymous type
        private IQueryable<ReservationReportItem> QueryReservations(ReportFilterDto f)
        {
            var q = _db.Reservations
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Bus!)   // Added null-forgiving operator
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.BusRoute!)  // Added null-forgiving operator
                .AsQueryable();

            if (f.From.HasValue) q = q.Where(r => r.Schedule.DepartureTime >= f.From.Value);
            if (f.To.HasValue) q = q.Where(r => r.Schedule.DepartureTime <= f.To.Value);

            if (f.RouteId.HasValue) q = q.Where(r => r.Schedule.BusRouteId == f.RouteId.Value);

            if (f.BusId.HasValue) q = q.Where(r => r.Schedule.BusId == f.BusId.Value);
            if (f.ScheduleId.HasValue) q = q.Where(r => r.ScheduleId == f.ScheduleId.Value);
            if (!string.IsNullOrWhiteSpace(f.CreatedBy)) q = q.Where(r => r.CreatedBy == f.CreatedBy);

            return q.Select(r => new ReservationReportItem
            {
                Id = r.Id,
                PassengerName = r.PassengerName,
                PassengerPhone = r.PassengerPhone,
                PassengerEmail = r.PassengerEmail,
                SeatNumber = r.SeatNumber,
                IsConfirmed = r.IsConfirmed,
                Departure = r.Schedule.DepartureTime,
                Arrival = r.Schedule.ArrivalTime,
                RouteFrom = r.Schedule.BusRoute!.StartLocation,  // Added null-forgiving operator
                RouteTo = r.Schedule.BusRoute!.EndLocation,      // Added null-forgiving operator
                BusNumber = r.Schedule.Bus!.BusNumber,           // Added null-forgiving operator
                CreatedAt = r.CreatedAt
            })
            .OrderBy(x => x.Departure);
        }

        // --- CSV -------------------------------------------------------------
        [HttpPost("reservations.csv")]
        public async Task<IActionResult> ExportReservationsCsv([FromBody] ReportFilterDto filter)
        {
            var rows = await QueryReservations(filter).ToListAsync();

            var sb = new StringBuilder();
            // header
            sb.AppendLine("Id,PassengerName,PassengerPhone,PassengerEmail,SeatNumber,IsConfirmed,Departure,Arrival,From,To,Bus,CreatedAt");

            foreach (var r in rows)
            {
                string Csv(string? v) => $"\"{(v ?? "").Replace("\"", "\"\"")}\"";
                string Bool(bool b) => b ? "TRUE" : "FALSE";
                string D(DateTime d) => d.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                sb.AppendLine(string.Join(",",
                    r.Id,
                    Csv(r.PassengerName),
                    Csv(r.PassengerPhone),
                    Csv(r.PassengerEmail),
                    Csv(r.SeatNumber),
                    Bool(r.IsConfirmed),
                    Csv(D(r.Departure)),
                    Csv(D(r.Arrival)),
                    Csv(r.RouteFrom),
                    Csv(r.RouteTo),
                    Csv(r.BusNumber),
                    Csv(D(r.CreatedAt))
                ));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"reservations_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
        }

        // --- PDF -------------------------------------------------------------
        [HttpPost("reservations.pdf")]
        public async Task<IActionResult> ExportReservationsPdf([FromBody] ReportFilterDto filter)
        {
            var rows = await QueryReservations(filter).ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Text("University Bus Reservation – Reservations Report")
                           .SemiBold().FontSize(14);
                        row.ConstantItem(120).Text(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                    });

                    page.Content().Table(table =>
                    {
                        // columns
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(36);  // Id
                            cols.RelativeColumn(1);   // Passenger
                            cols.ConstantColumn(70);  // Phone
                            cols.ConstantColumn(60);  // Seat
                            cols.ConstantColumn(110); // Departure
                            cols.RelativeColumn(1);   // Route
                            cols.ConstantColumn(50);  // Bus
                            cols.ConstantColumn(70);  // Status
                        });

                        // header - Fixed: Now using proper method chaining
                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderCell).Text("ID");
                            h.Cell().Element(HeaderCell).Text("Passenger");
                            h.Cell().Element(HeaderCell).Text("Phone");
                            h.Cell().Element(HeaderCell).Text("Seat");
                            h.Cell().Element(HeaderCell).Text("Departure");
                            h.Cell().Element(HeaderCell).Text("Route");
                            h.Cell().Element(HeaderCell).Text("Bus");
                            h.Cell().Element(HeaderCell).Text("Status");
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Element(Cell).Text(r.Id.ToString());
                            table.Cell().Element(Cell).Text(r.PassengerName ?? "");
                            table.Cell().Element(Cell).Text(r.PassengerPhone ?? "");
                            table.Cell().Element(Cell).Text(r.SeatNumber ?? "");
                            table.Cell().Element(Cell).Text(r.Departure.ToString("dd MMM yyyy HH:mm"));
                            table.Cell().Element(Cell).Text($"{r.RouteFrom ?? ""} → {r.RouteTo ?? ""}");
                            table.Cell().Element(Cell).Text(r.BusNumber ?? "");
                            table.Cell().Element(Cell).Text(r.IsConfirmed ? "Confirmed" : "Pending");
                        }

                        // Helper methods - simplified
                        static IContainer HeaderCell(IContainer c) =>
                            c.PaddingVertical(4).BorderBottom(1).DefaultTextStyle(x => x.SemiBold());

                        static IContainer Cell(IContainer c) =>
                            c.PaddingVertical(3).BorderBottom(0.5f).DefaultTextStyle(x => x.FontSize(9));
                    });

                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Generated: ").SemiBold();
                        txt.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                    });
                });
            });

            byte[] pdf = doc.GeneratePdf();
            return File(pdf, "application/pdf", $"reservations_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf");
        }
    }

    // Strongly-typed class for the report data
    public class ReservationReportItem
    {
        public int Id { get; set; }
        public string? PassengerName { get; set; }
        public string? PassengerPhone { get; set; }
        public string? PassengerEmail { get; set; }
        public string? SeatNumber { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public string? RouteFrom { get; set; }
        public string? RouteTo { get; set; }
        public string? BusNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}