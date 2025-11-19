using Microsoft.AspNetCore.Mvc;

namespace Software_Engineering_2025.Controllers
{
    /// <summary>
    /// Handles navigation and page rendering for admin interface.
    /// Separate from AdminController which handles API requests.
    /// </summary>
    public class AdminViewController : Controller
    {
        /// <summary>
        /// Admin dashboard page (shown after login)
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new user form page (User Story 1)
        /// </summary>
        public IActionResult CreateUser()
        {
            return View();
        }

        /// <summary>
        /// Manage users list page (User Story 2)
        /// </summary>
        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}