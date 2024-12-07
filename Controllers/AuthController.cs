using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.Models;
using System.Threading.Tasks;

namespace StudyGroupFinder.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            // Check if the user already exists (you can expand the validation)
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            var user = new ApplicationUser { UserName = registerRequest.Email, Email = registerRequest.Email };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }

            return BadRequest(new { message = "Registration failed", errors = result.Errors });
        }


        // POST: api/auth/login (existing)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
        {
            if (loginModel == null)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful" });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }
    }

    // Model for Register Request
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Model for Login Request (already existing)
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
