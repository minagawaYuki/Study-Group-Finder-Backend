using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroupFinder;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;
using StudyGroupFinder.Services;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudyGroupController : ControllerBase
{
    private readonly StudyGroupFinderDbContext _context;
    private readonly IStudyGroupService _studyGroupService;

    public StudyGroupController(StudyGroupFinderDbContext context, IStudyGroupService studyGroupService)
    {
        _context = context;
        _studyGroupService = studyGroupService;
    }

    [HttpGet("all")]
    [AllowAnonymous] // If you want unauthenticated users to view study groups
    public async Task<IActionResult> GetAllStudyGroups()
    {
        var studyGroups = await _studyGroupService.GetAllStudyGroupsAsync();

        if (studyGroups == null || !studyGroups.Any())
            return NotFound(new { Message = "No study groups found" });

        return Ok(studyGroups);
    }

    [HttpGet("details/{groupId}")]
    public async Task<IActionResult> GetGroupDetails(int groupId)
    {
        var group = await _studyGroupService.GetGroupDetailsAsync(groupId);

        if (group == null)
            return NotFound(new { Message = "Group not found" });

        return Ok(group);
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] StudyGroupDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized(new { Message = "Unauthorized access" });

        // Fetch the user from the database
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return BadRequest(new { Message = "User not found" });
        }

        var studyGroup = new StudyGroup
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Subject = createDto.Subject,
            CourseCode = createDto.CourseCode,
            CreatedByUserId = userId,  // Set CreatedByUserId
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user  // Set CreatedBy to the fetched user
        };

        // Create the study group in the database
        var result = await _studyGroupService.CreateGroupAsync(studyGroup, userId);

        if (result)
        {
            // Add the user as a member of the newly created group
            var groupMember = new GroupMember
            {
                StudyGroupId = studyGroup.Id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();  // Save changes to include the user as a member

            return Ok(new { Message = "Study group created and you are now a member." });
        }

        return BadRequest(new { Message = "Failed to create study group" });
    }





    [HttpPost("join/{groupId}")]
    public async Task<IActionResult> JoinGroup(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized(new { Message = "Unauthorized access" });

        var result = await _studyGroupService.JoinGroupAsync(groupId, userId);

        if (result)
            return Ok(new { Message = "Successfully joined the study group" });

        return BadRequest(new { Message = "Failed to join the study group. The group might not exist or you may already be a member." });
    }
}
