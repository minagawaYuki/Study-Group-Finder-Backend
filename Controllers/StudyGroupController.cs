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

    [Authorize]
    [HttpPost("leave/{groupId}")]
    public async Task<IActionResult> LeaveGroup(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the authenticated user's ID

        // Find the study group by ID
        var studyGroup = await _context.StudyGroups
                                        .Include(g => g.Members)
                                        .ThenInclude(m => m.User)
                                        .FirstOrDefaultAsync(g => g.Id == groupId);

        if (studyGroup == null)
        {
            return NotFound(new { message = "Study group not found." });
        }

        // Find the user in the group
        var groupMember = studyGroup.Members.FirstOrDefault(m => m.UserId == userId);

        if (groupMember == null)
        {
            return BadRequest(new { message = "You are not a member of this group." });
        }

        // Remove the user from the group
        studyGroup.Members.Remove(groupMember);

        // Save the changes to the database
        await _context.SaveChangesAsync();

        return Ok(new { message = "You have successfully left the group." });
    }

    // GET: api/studygroup/joined
    [HttpGet("joined")]
    [Authorize] // Ensure the user is authenticated
    public async Task<ActionResult> GetUserJoinedGroups()
    {
        // Get the UserId from the JWT token claim
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var joinedGroups = await _context.GroupMembers
            .Where(gm => gm.UserId == userId) // Filter by the current user
            .Include(gm => gm.StudyGroup) // Include the related StudyGroup
            .Select(gm => new
            {
                gm.StudyGroup.Id,
                gm.StudyGroup.Name,
                gm.StudyGroup.Description,
                gm.StudyGroup.Subject,
                gm.StudyGroup.CourseCode,
                gm.StudyGroup.CreatedAt
            })
            .ToListAsync();

        if (joinedGroups == null || !joinedGroups.Any())
        {
            return NotFound("No study groups found for this user.");
        }

        return Ok(joinedGroups);
    }

    // Controller action to check if the user is a member of a study group
    [HttpGet("IsUserMember/{groupId}")]
    public async Task<IActionResult> IsUserMember(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
        var isMember = await _context.GroupMembers
                                     .AnyAsync(gm => gm.UserId == userId && gm.StudyGroupId == groupId);

        return Ok(isMember);
    }

    // Endpoint to post an announcement (Group Owner only)
    [HttpPost("{groupId}/announcements")]
    [Authorize]
    public async Task<IActionResult> PostAnnouncement(int groupId, [FromBody] string content)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var group = await _context.StudyGroups.Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return NotFound("Group not found.");

        if (group.CreatedByUserId != userId)
            return StatusCode(StatusCodes.Status403Forbidden, "Only group owners can post announcements.");

        var announcement = new Announcement
        {
            StudyGroupId = groupId,
            Content = content,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return Ok(announcement);
    }

    // Endpoint to comment on an announcement (Members only)
    [HttpPost("announcements/{announcementId}/comments")]
    [Authorize]
    public async Task<IActionResult> PostComment(int announcementId, [FromBody] string content)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var announcement = await _context.Announcements.Include(a => a.StudyGroup)
            .ThenInclude(g => g.Members)
            .FirstOrDefaultAsync(a => a.Id == announcementId);

        if (announcement == null)
            return NotFound("Announcement not found.");

        var isMember = announcement.StudyGroup.Members.Any(m => m.UserId == userId);
        if (!isMember)
            return StatusCode(StatusCodes.Status403Forbidden, "Only group members can comment.");

        var comment = new Comment
        {
            AnnouncementId = announcementId,
            Content = content,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }


    // Endpoint to get announcements and their comments for a group
    [HttpGet("{groupId}/announcements")]
    public async Task<IActionResult> GetAnnouncements(int groupId)
    {
        var announcements = await _context.Announcements
            .Where(a => a.StudyGroupId == groupId)
            .Include(a => a.Comments)
            .ToListAsync();

        return Ok(announcements);
    }
}
