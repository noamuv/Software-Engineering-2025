// This controller handles requests to the welcome page of the application. 


using Microsoft.AspNetCore.Mvc;
namespace Software_Engineering_2025.Controllers
// Namespace organizes classes and prevents naming conflicts.
{
    public class WelcomeController : Controller
    // The WelcomeController class inherits from the base Controller class provided by ASP.NET Core MVC.
    {
        public IActionResult Index()
        // The Index method handles GET requests to the /Welcome/Index URL.
        {
            return View();
            // Returns the default view associated with this action method.
        }
        /*  The following methods redirect to the Login controller's Index action.
        public IActionResult Patient()   => RedirectToAction("Index", "Login");
        public IActionResult Clinician() => RedirectToAction("Index", "Login");
        public IActionResult Carer()     => RedirectToAction("Index", "Login");
        public IActionResult Admin()     => RedirectToAction("Index", "Login");
        */
    }
}