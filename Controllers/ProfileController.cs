using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("get-profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized(new { Message = "Unauthorized access" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound(new { Message = "User not found" });

        return Ok(new
        {
            FullName = user.FullName,
            Bio = user.Bio
        });
    }

    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized(new { Message = "Unauthorized access" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound(new { Message = "User not found" });

        user.FullName = profileDto.FullName ?? user.FullName;
        user.Bio = profileDto.Bio ?? user.Bio;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Profile updated successfully" });
        }

        return BadRequest(new { Message = "Failed to update profile", Errors = result.Errors });
    }
}
