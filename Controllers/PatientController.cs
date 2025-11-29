using Microsoft.AspNetCore.Mvc;
using Software_Engineering_2025.Models;
using System;
using System.Linq;

namespace Software_Engineering_2025.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PressureDataService _pressureDataService;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DASHBOARD - Patient Overview
        
        [HttpGet]
        public IActionResult Dashboard(Guid userId)
        {
            // Get patient from database
            var patient = _context.AppUsers.Find(userId);
            
            if (patient == null || patient.Role != "Patient")
            {
                return RedirectToAction("Index", "Welcome");
            }

            

            var pressureData = _pressureDataService.GetLatestPressureData(userId);
            var metrics = PressureMatrix.CalculateMetrics();

            ViewBag.PressureMatrix = PressureMatrix;
            ViewBag.Metrics = metrics;

            // Pass patient to view
            return View(patient);
        }

      
    }
}