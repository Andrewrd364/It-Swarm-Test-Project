using It_Swarm_Test_Project.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace It_Swarm_Test_Project.Controllers
{
    public class MetersController : Controller
    {
        private readonly MeterContext _context;

        public MetersController(MeterContext context)
        {
            _context = context;
        }

        // GET: Meters
        public async Task<IActionResult> Index(string? house, string? street)
        {
            var meters = await _context.Apartments
                .Include(a => a.CurrentMeter)
                .ThenInclude(m => m.Readings)
                .ToListAsync();

            if (!string.IsNullOrEmpty(street))
            {
                meters = meters.Where(a => a.Street == street).ToList();
            }
            if (!string.IsNullOrEmpty(house))
            {
                meters = meters.Where(a => a.House == house).ToList();
            }

            meters = meters.Where(a => a.CurrentMeter != null && a.CurrentMeter.NextVerificationDate <= DateTime.UtcNow).ToList();

            return View(meters);
        }
        // GET: Meters/AddReading/5
        public async Task<IActionResult> AddReading(int? apartmentId)
        {
            if (apartmentId == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.CurrentMeter)
                .FirstOrDefaultAsync(a => a.Id == apartmentId);

            if (apartment == null || apartment.CurrentMeter == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Meters/AddReading/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReading(int apartmentId, decimal newReadingValue, DateTime nextVerificationDate)
        {
            var apartment = await _context.Apartments
                .Include(a => a.CurrentMeter)
                .ThenInclude(m => m.Readings)
                .FirstOrDefaultAsync(a => a.Id == apartmentId);

            if (apartment == null || apartment.CurrentMeter == null)
            {
                return NotFound();
            }

            var currentMeter = apartment.CurrentMeter;

            var lastReading = currentMeter.Readings?.OrderByDescending(r => r.ReadingDate).FirstOrDefault();
            if (lastReading != null && newReadingValue <= lastReading.Value)
            {
                ModelState.AddModelError("", $"Новое показание должно быть больше предыдущего. Предыдущее показание: {lastReading.Value}.");
                return View(apartment);
            }

            if (nextVerificationDate <= DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Дата следующей поверки должна быть в будущем.");
                return View(apartment);
            }

            nextVerificationDate = DateTime.SpecifyKind(nextVerificationDate, DateTimeKind.Utc);

            var newMeterReading = new MeterReading
            {
                MeterId = currentMeter.Id,   
                Meter = currentMeter,       
                Value = newReadingValue,
                ReadingDate = DateTime.UtcNow
            };

            _context.MeterReadings.Add(newMeterReading);

            currentMeter.LastVerificationDate = DateTime.UtcNow;
            currentMeter.NextVerificationDate = nextVerificationDate;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
