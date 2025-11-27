// This controller handles requests to the welcome page of the application. 


using Microsoft.AspNetCore.Mvc;
namespace Software_Engineering_2025.Controllers
// Namespace organizes classes and prevents naming conflicts.
{
    // The WelcomeController class inherits from the base Controller class provided by ASP.NET Core MVC.
    public class WelcomeController : Controller
    {
        // The Index method handles GET requests to the /Welcome/Index URL.
        public IActionResult Index()
       
        {
        // This returns the default view associated with this action method.
            return View();
           
        }
   
    }
}