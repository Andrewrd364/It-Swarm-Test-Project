using It_Swarm_Test_Project.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace It_Swarm_Test_Project.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly MeterContext _context;

        public ApartmentsController(MeterContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? street, string? house)
        {
            var apartments = await _context.Apartments
        .Include(a => a.CurrentMeter)
        .ThenInclude(m => m.Readings)
        .ToListAsync();

            if (!string.IsNullOrEmpty(street))
            {
                apartments = apartments.Where(a => a.Street == street).ToList();
            }

            if (!string.IsNullOrEmpty(house))
            {
                apartments = apartments.Where(a => a.House == house).ToList();
            }

            return View(apartments);
        }
        // GET: Отображение формы для добавления/замены счётчика
        [HttpGet]
        public IActionResult ReplaceMeter(int id)
        {
            var apartment = _context.Apartments
                .Include(a => a.CurrentMeter)
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Сохранение нового или заменённого счётчика
        [HttpPost]
        public async Task<IActionResult> ReplaceMeter(int apartmentId, string newMeterSerialNumber, DateTime nextVerificationDate)
        {
            var apartment = await _context.Apartments
            .Include(a => a.CurrentMeter)
            .ThenInclude(m => m.Readings) 
            .FirstOrDefaultAsync(a => a.Id == apartmentId);

            if (apartment == null)
            {
                return NotFound();
            }

            var newMeter = new Meter
            {
                SerialNumber = newMeterSerialNumber,
                LastVerificationDate = DateTime.UtcNow, 
                NextVerificationDate = DateTime.SpecifyKind(nextVerificationDate, DateTimeKind.Utc) 
            };

            _context.Meters.Add(newMeter);
            await _context.SaveChangesAsync();

            var initialReading = new MeterReading
            {
                MeterId = newMeter.Id,
                Meter = newMeter,
                Value = 0, 
                ReadingDate = DateTime.UtcNow 
            };

            _context.MeterReadings.Add(initialReading);
            await _context.SaveChangesAsync();

            var replacementHistory = new MeterReplacementHistory
            {
                ApartmentId = apartment.Id,
                Apartment = apartment,
                NewMeterId = newMeter.Id,
                InstallationDate = DateTime.UtcNow,
                PreviousMeterReadingId = apartment.LatestReading?.Id,
                PreviousMeterReading = apartment.LatestReading
            };

            _context.MeterReplacementHistories.Add(replacementHistory);

            apartment.CurrentMeterId = newMeter.Id;

            await _context.SaveChangesAsync(); 

            return RedirectToAction("Index", "Apartments");
        }
        [HttpGet]
        public async Task<IActionResult> GetReplacementHistory(int apartmentId)
        {
            var replacementHistory = await _context.MeterReplacementHistories
                .Where(h => h.ApartmentId == apartmentId)
                .Include(h => h.NewMeter)
                .Include(h => h.PreviousMeterReading)
                .Select(h => new
                {
                    InstallationDate = h.InstallationDate,
                    NewMeterSerialNumber = h.NewMeter != null ? h.NewMeter.SerialNumber : null,
                    NextVerificationDate = h.NewMeter != null ? h.NewMeter.NextVerificationDate : (DateTime?)null,
                    PreviousReadingValue = h.PreviousMeterReading != null ? (decimal?)h.PreviousMeterReading.Value : null,
                    PreviousReadingDate = h.PreviousMeterReading != null ? (DateTime?)h.PreviousMeterReading.ReadingDate : null
                })
                .OrderByDescending(h => h.InstallationDate)
                .ToListAsync();

            return Json(replacementHistory);
        }
    }
}
