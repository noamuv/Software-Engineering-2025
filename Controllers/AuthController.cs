using Microsoft.AspNetCore.Mvc;
using Software_Engineering_2025.Services;
using System;

// This controller manages user authentication, including sign-in for first-time users,
// login for returning users, password resets, and logout functionality.
namespace Software_Engineering_2025.Controllers
{
    public class AuthController : Controller
    {
        /* Dependency injection of the AuthenticationService
        AuthenticationService handles the core authentication logic.
         It interacts with the database and validates user credentials.
         */
        private readonly AuthenticationService _authService;
        
        // Constructor to initialize the AuthenticationController with the AuthenticationService
        public AuthController(AuthenticationService authService)
        {
            // Injected authentication service
            _authService = authService;
        }

    
        // SIGN IN - First Time Users (Temporary Password)
    
        
        [HttpGet]
        public IActionResult SignIn(string? role)
        {
            // Pass the role to the view for display purposes
            ViewBag.Role = role ?? "User";
            return View();
        }

        [HttpPost]
        // Method to handle sign-in with temporary password
        public IActionResult SignIn(string email, string temporaryPassword, string? role)
        {
            // Authenticate user with temporary password
            var result = _authService.AuthenticateWithTemporaryPassword(email, temporaryPassword);

             // If authentication fails, return to sign-in view with error message
            if (!result.IsSuccessful)
            {
                ViewBag.Role = role ?? "User";
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            // Redirect to reset password for first-time users
            return RedirectToAction("ResetPassword", new { userId = result.User!.Id });
        }

        // LOGIN - Returning Users (Signing in with Regular Password)
        
        [HttpGet]
        public IActionResult Login(string? role)
        {
            ViewBag.Role = role ?? "User";
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string? role)
        {
            var ptest_result = _authService.AuthenticateWithPassword(email, password);

            if (!ptest_result.IsSuccessful)
            {
                ViewBag.Role = role ?? "User";
                ViewBag.Error = ptest_result.ErrorMessage;
                return View();
            }

            // Redirect to appropriate dashboard based on role
            return RedirectToDashboard(ptest_result.User!.Role);
        }

   
        // RESET PASSWORD - After First Sign In
     
        
        [HttpGet]
        public IActionResult ResetPassword(Guid userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(Guid userId, string newPassword, string confirmPassword)
        {
            var ptest_result = _authService.ResetPassword(userId, newPassword, confirmPassword);

            if (!ptest_result.IsSuccessful)
            {
                ViewBag.UserId = userId;
                ViewBag.Error = ptest_result.ErrorMessage;
                return View();
            }

            // Show success message and redirect to login
            TempData["SuccessMessage"] = "Password set successfully! Please login with your new password.";
            return RedirectToAction("Login");
        }

        
        // LOGOUT functionality
        
        public IActionResult Logout()
        {
            // Clear session/cookies here
            return RedirectToAction("Index", "Welcome");
        }

      
        //Redirects to Dashboard Based on Role
        
        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Patient" => RedirectToAction("Index", "PatientDashboard"),
                "Clinician" => RedirectToAction("Index", "ClinicianDashboard"),
                "Carer" => RedirectToAction("Index", "CarerDashboard"),
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                _ => RedirectToAction("Index", "Welcome")
            };
        }
    }
}