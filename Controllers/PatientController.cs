using Microsoft.AspNetCore.Mvc;
using Software_Engineering_2025.Models;
using Software_Engineering_2025.Services;
using System;
using System.Linq;

namespace Software_Engineering_2025.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly PressureDataService _pressureDataService;
        private readonly CsvImportService _csvImport;

        public PatientController(ApplicationDbContext context, CsvImportService csvImport)
        {
            _context = context;
            _csvImport = csvImport;
        }

        // DASHBOARD - Patient Overview
        
        [HttpGet]

           public IActionResult Dashboard(Guid userId)
        {
            // Get patient info
            var patient = _context.AppUsers.Find(userId);
            
            if (patient == null || patient.Role != "Patient")
            {
                return RedirectToAction("Index", "Welcome");
            }

            // Get ALL pressure sessions for this patient
            var sessions = _context.PressureSessions
                .Where(p => p.PatientUserId == patient.CsvUserId)
                .OrderByDescending(p => p.RecordedDate)
                .ToList();

            if (sessions.Count == 0)
            {
                // No data available
                ViewBag.NoData = true;
                return View(patient);
            }

            // Get latest session by default
            var latestSession = sessions.First();
            
            // Deserialize matrix for heat map
            var matrix = _csvImport.DeserializeMatrix(latestSession.MatrixJson);

            // Pass data to view
            ViewBag.Patient = patient;
            ViewBag.AvailableSessions = sessions; // All dates
            ViewBag.CurrentSession = latestSession; // Currently displayed
            ViewBag.Matrix = matrix;
            ViewBag.Metrics = new {
                PeakPressure = latestSession.PeakPressure,
                ContactAreaPercent = latestSession.ContactAreaPercent,
                CoefficientOfVariation = latestSession.CoefficientOfVariation,
                RiskScore = latestSession.RiskScore
            };
            
            return View(patient);
        }

      [HttpGet]
public IActionResult GetPressureData(Guid userId, string date)
{
    try
    {
        Console.WriteLine($"📥 GetPressureData called: userId={userId}, date={date}");
        
        // Get patient
        var patient = _context.AppUsers.Find(userId);
        if (patient == null)
        {
            Console.WriteLine($"❌ Patient not found: {userId}");
            return NotFound(new { error = "Patient not found" });
        }

        Console.WriteLine($"✅ Found patient: {patient.Email}, CsvUserId={patient.CsvUserId}");

        // Check if patient has CsvUserId
        if (string.IsNullOrEmpty(patient.CsvUserId))
        {
            Console.WriteLine($"❌ Patient has no CsvUserId");
            return BadRequest(new { error = "Patient has no pressure data linked" });
        }

        // Parse date
        if (!DateTime.TryParse(date, out DateTime selectedDate))
        {
            Console.WriteLine($"❌ Invalid date format: {date}");
            return BadRequest(new { error = "Invalid date format" });
        }

        Console.WriteLine($"✅ Parsed date: {selectedDate:yyyy-MM-dd}");

        // Find session
        var session = _context.PressureSessions
            .Where(p => p.PatientUserId == patient.CsvUserId 
                     && p.RecordedDate.Date == selectedDate.Date)
            .FirstOrDefault();

        if (session == null)
        {
            Console.WriteLine($"❌ No session found for {patient.CsvUserId} on {selectedDate:yyyy-MM-dd}");
            
            // List available dates for debugging
            var availableDates = _context.PressureSessions
                .Where(p => p.PatientUserId == patient.CsvUserId)
                .Select(p => p.RecordedDate.Date)
                .Distinct()
                .ToList();
            
            Console.WriteLine($"   Available dates: {string.Join(", ", availableDates.Select(d => d.ToString("yyyy-MM-dd")))}");
            
            return NotFound(new { error = "No data available for selected date" });
        }

        Console.WriteLine($"✅ Found session: {session.Id}");

        // Deserialize matrix (with error handling)
        int[,] matrix;
        try
        {
            matrix = _csvImport.DeserializeMatrix(session.MatrixJson);
            Console.WriteLine($"✅ Matrix deserialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to deserialize matrix: {ex.Message}");
            return StatusCode(500, new { error = "Failed to load pressure data" });
        }

        // Build response
        var response = new
        {
            date = session.RecordedDate.ToString("MMM dd, yyyy"),
            matrix = matrix,
            metrics = new
            {
                peakPressure = session.PeakPressure,
                contactArea = session.ContactAreaPercent,
                cv = session.CoefficientOfVariation,
                riskScore = session.RiskScore
            }
        };

        Console.WriteLine($"✅ Returning data: Peak={response.metrics.peakPressure}, Contact={response.metrics.contactArea}%");
        
        return Json(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Unexpected error in GetPressureData: {ex.Message}");
        Console.WriteLine($"   Stack trace: {ex.StackTrace}");
        return StatusCode(500, new { error = "Internal server error", details = ex.Message });
    }
}
    }
}