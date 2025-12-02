using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Software_Engineering.Data;

namespace Software_Engineering.Controllers
{
    public class CarerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Hardcoded for testing - NOT real world! We will pass the value dynamically when implementing the authentication
            int carerUserId = 1;

            // Fetch carer accesses based on carerID and return the patient(s) and user(s) details
            var carerAccesses = await _context.Carer_Accesses
                .Where(ca => ca.Carer_User_ID == carerUserId && ca.Revoked_At == null)
                .Include(ca => ca.Patient)
                .ThenInclude(p => p.User)
                .ToListAsync();

            return View(carerAccesses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            //Check if id was provided
            if (id == null)
            {
                return NotFound();
            }

            //Hardcoded Carer ID for testing
            int carerUserId = 1;

            //Fetch patient details, but only if carer has access
            var carerAccess = await _context.Carer_Accesses
                .Where(ca => ca.Carer_User_ID == carerUserId && ca.Patient_User_ID == id && ca.Revoked_At == null)
                .Include(ca => ca.Patient)
                    .ThenInclude(p => p.User)
                .Include(ca => ca.Carer)
                    .ThenInclude(c => c.User)
                //Gets first item from database that matches query or null if no items match
                .FirstOrDefaultAsync();

            if (carerAccess == null)
            {
                return NotFound($"No access found for Carer {carerUserId} and Patient {id}");
            }
            return View(carerAccess);

        }
    }
    
}