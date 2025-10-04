using System;
using System.Linq;
using BusReservation.API.Models;

namespace BusReservation.API.Data
{
    public static class SeedData
    {
        public static void Run(ApplicationDbContext db)
        {
            // Already seeded?
            if (db.Schedules.Any()) return;

            // 1️⃣ Create routes
            var route1 = new BusRoute
            {
                StartLocation = "Campus",
                EndLocation = "City Center",
                DistanceInKm = 12.5,
                EstimatedTime = TimeSpan.FromMinutes(30)
            };

            var route2 = new BusRoute
            {
                StartLocation = "City Center",
                EndLocation = "Campus",
                DistanceInKm = 12.5,
                EstimatedTime = TimeSpan.FromMinutes(35)
            };

            // 2️⃣ Create buses
            var bus1 = new Bus
            {
                BusNumber = "Bus-101",
                BusType = "AC",
                Capacity = 40,                  // ✅ use Capacity instead of TotalSeats
                PlateNumber = "UP14AB1234",
                IsActive = true
            };

            var bus2 = new Bus
            {
                BusNumber = "Bus-102",
                BusType = "Non-AC",
                Capacity = 35,
                PlateNumber = "UP14CD5678",
                IsActive = true
            };

            // 3️⃣ Create drivers
            var campusDriver = new Driver
            {
                FullName = "Amit Kumar",
                LicenseNumber = "DL12345",
                PhoneNumber = "9876543210",
                ExperienceYears = 5,
                IsActive = true
            };

            var cityCenterDriver = new Driver
            {
                FullName = "Suresh Singh",
                LicenseNumber = "DL67890",
                PhoneNumber = "9123456780",
                ExperienceYears = 8,
                IsActive = true
            };


            db.BusRoutes.AddRange(route1, route2);
            db.Buses.AddRange(bus1, bus2);
            db.Drivers.AddRange(campusDriver, cityCenterDriver);
            db.SaveChanges();

            // 4️⃣ Create schedules
            var schedule1 = new Schedule
            {
                BusRouteId = route1.Id,
                BusId = bus1.Id,
                DriverId = campusDriver.Id,
                DepartureTime = DateTime.Today.AddHours(9),
                ArrivalTime = DateTime.Today.AddHours(11)
            };

            var schedule2 = new Schedule
            {
                BusRouteId = route2.Id,
                BusId = bus2.Id,
                DriverId = cityCenterDriver.Id,
                DepartureTime = DateTime.Today.AddHours(17),
                ArrivalTime = DateTime.Today.AddHours(19)
            };

            db.Schedules.AddRange(schedule1, schedule2);
            db.SaveChanges();

            // 5️⃣ Example reservation
            db.Reservations.Add(new Reservation
            {
                ScheduleId = schedule1.Id,
                PassengerName = "Rahul",
                PassengerPhone = "9999999999",
                SeatNumber = "A1"
                // Status = "Pending"  ✅ only if Reservation model has this field
            });

            db.SaveChanges();
        }
    }
}
