using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.Models;
using System.Security.Claims;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET api/profile/get-profile
    [HttpGet("get-profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from token
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            FullName = $"{user.FirstName} {user.LastName}",
            YearLevel = user.YearLevel ?? "Year not set",
            Course = user.Course ?? "Course not set",
            Description = user.Description ?? "No description provided"
        });
    }

    // POST api/profile/update-profile
    [HttpPost("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.YearLevel = model.YearLevel;
        user.Course = model.Course;
        user.Description = model.Description;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "Profile updated successfully" });
        }

        return BadRequest(result.Errors);
    }
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string YearLevel { get; set; }
    public string Course { get; set; }
    public string Description { get; set; }
}
