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

        public IActionResult GetPressureData(Guid userId, string date)
        {
            var patient = _context.AppUsers.Find(userId);
            
            if (patient == null || !DateTime.TryParse(date, out DateTime selectedDate))
            {
                return BadRequest();
            }

            var session = _context.PressureSessions
                .Where(p => p.PatientUserId == patient.CsvUserId 
                         && p.RecordedDate.Date == selectedDate.Date)
                .FirstOrDefault();

            if (session == null)
            {
                return NotFound();
            }

            // Deserialize matrix
            var matrix = _csvImport.DeserializeMatrix(session.MatrixJson);

            return Json(new
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
            });
        }
    }
}