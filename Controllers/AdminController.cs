using Microsoft.AspNetCore.Mvc;
using Software_Engineering_2025.DTOs;
using Software_Engineering_2025.Services;

namespace Software_Engineering_2025.Controllers
{
    /// <summary>
    /// Handles admin operations for user account management.
    /// Implements user stories for creating and managing user accounts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user account. Validates email uniqueness and hashes password.
        /// User Story: "As an Admin, I want to create user accounts"
        /// </summary>
        [HttpPost("create-user")]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserCreateRequest request)
        {
            try
            {
                // Validate email doesn't already exist
                if (await _userService.EmailExistsAsync(request.Email))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Create the user
                var user = await _userService.CreateUserAsync(
                    request.User_Type_ID,
                    request.First_Name,
                    request.Last_Name,
                    request.Email,
                    request.Password,
                    request.Title
                );

                // Map to response DTO (exclude sensitive data)
                var response = new UserResponse
                {
                    User_ID = user.User_ID,
                    User_Type_ID = user.User_Type_ID,
                    User_Type_Name = user.UserType?.Type_Name,
                    Title = user.Title,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Email = user.Email,
                    Status = user.Status,
                    Created_At = user.Created_At,
                    Is_Activated = user.Is_Activated
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.User_ID }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Disables a user account by setting status to "Disabled".
        /// User Story: "As an Admin, I want to disable user accounts"
        /// </summary>
        [HttpPut("disable-user/{id}")]
        public async Task<IActionResult> DisableUser(int id)
        {
            try
            {
                var result = await _userService.DisableUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User disabled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Re-enables a previously disabled user account.
        /// </summary>
        [HttpPut("enable-user/{id}")]
        public async Task<IActionResult> EnableUser(int id)
        {
            try
            {
                var result = await _userService.EnableUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User enabled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific user by ID. Includes user type information.
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Map to response DTO
                var response = new UserResponse
                {
                    User_ID = user.User_ID,
                    User_Type_ID = user.User_Type_ID,
                    User_Type_Name = user.UserType?.Type_Name,
                    Title = user.Title,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Email = user.Email,
                    Status = user.Status,
                    Created_At = user.Created_At,
                    Is_Activated = user.Is_Activated
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all users in the system. Useful for admin dashboard.
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                // Map to response DTOs
                var response = users.Select(user => new UserResponse
                {
                    User_ID = user.User_ID,
                    User_Type_ID = user.User_Type_ID,
                    User_Type_Name = user.UserType?.Type_Name,
                    Title = user.Title,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Email = user.Email,
                    Status = user.Status,
                    Created_At = user.Created_At,
                    Is_Activated = user.Is_Activated
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
    }
}