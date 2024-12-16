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

    [HttpDelete("{groupId}")]
    public async Task<IActionResult> DeleteStudyGroup(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Fetch the study group, including its members and related entities if necessary
        var group = await _context.StudyGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return NotFound(new { message = "Study group not found." });

        // Check if the current user is the creator of the group
        if (group.CreatedByUserId != userId)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not authorized to delete this study group." });

        // Remove the group
        _context.StudyGroups.Remove(group);

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok(new { message = "Study group deleted successfully." });
    }

    // Edit Study Group
    [HttpPut("{groupId}")]
    public async Task<IActionResult> EditGroup(int groupId, [FromBody] StudyGroupDto editGroupDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Fetch the group and verify ownership
        var group = await _context.StudyGroups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            return NotFound("Group not found.");

        if (group.CreatedByUserId != userId)
            return Forbid(); // Forbid doesn't accept an anonymous object, so remove the custom message

        // Update group properties
        group.Name = editGroupDto.Name;
        group.Description = editGroupDto.Description;
        group.Subject = editGroupDto.Subject;
        group.CourseCode = editGroupDto.CourseCode;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Group updated successfully." });
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





    [Authorize]
    [HttpPost("group/members/{groupId}")]
    public async Task<IActionResult> JoinGroup(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new { Message = "Unauthorized access" });

        var result = await _studyGroupService.JoinGroupAsync(groupId, userId);

        if (result)
            return Ok(new { Message = "Successfully joined the study group" });

        return BadRequest(new { Message = "Failed to join the study group. The group might not exist, or you may already be a member." });
    }

    [Authorize]
    [HttpDelete("group/members/{groupId}")]
    public async Task<IActionResult> LeaveGroup(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Fetch the study group along with its members
        var studyGroup = await _context.StudyGroups
                                        .Include(g => g.Members)
                                        .FirstOrDefaultAsync(g => g.Id == groupId);

        if (studyGroup == null)
            return NotFound(new { Message = "Study group not found." });

        // Check if the user is a member of the group
        var groupMember = studyGroup.Members.FirstOrDefault(m => m.UserId == userId);
        if (groupMember == null)
            return BadRequest(new { Message = "You are not a member of this group." });

        // Check if the user is the group owner
        if (studyGroup.CreatedByUserId == userId)
        {
            // Delete the group if the owner leaves
            _context.StudyGroups.Remove(studyGroup);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { Message = "As the owner, you have left and the group has been deleted." });
        }
        else
        {
            // Remove the user from the group
            studyGroup.Members.Remove(groupMember);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { Message = "You have successfully left the group." });
        }
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
    public async Task<IActionResult> PostAnnouncement(int groupId, [FromBody] PostAnnouncementDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Content cannot be empty.");

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
            Content = dto.Content,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return Ok(announcement);
    }

    [HttpPut("announcements/{announcementId}")]
    public async Task<IActionResult> UpdateAnnouncement(int announcementId, [FromBody] UpdateAnnouncementDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var announcement = await _context.Announcements.FindAsync(announcementId);

        if (announcement == null)
            return NotFound(new { Message = "Announcement not found." });

        if (announcement.CreatedByUserId != userId)
            return Forbid();

        announcement.Content = dto.Content;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Announcement updated successfully." });
    }

    // Delete an announcement
    [HttpDelete("announcements/{announcementId}")]
    public async Task<IActionResult> DeleteAnnouncement(int announcementId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Fetch the announcement and the related group
        var announcement = await _context.Announcements
            .Include(a => a.StudyGroup)
            .FirstOrDefaultAsync(a => a.Id == announcementId);

        if (announcement == null)
            return NotFound(new { message = "Announcement not found." });

        // Check if the user is the owner of the group
        if (announcement.StudyGroup.CreatedByUserId != userId)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not authorized to delete this announcement." });

        // Delete the announcement
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Announcement deleted successfully." });
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


    [HttpGet("{groupId}/announcements")]
    public async Task<IActionResult> GetAnnouncements(int groupId)
    {
        var announcements = await _context.Announcements
            .Where(a => a.StudyGroupId == groupId)
            .Include(a => a.Comments)
            .Select(a => new
            {
                a.Id,
                a.Content,
                a.CreatedAt,
                CreatedBy = _context.Users
                    .Where(u => u.Id == a.CreatedByUserId)
                    .Select(u => u.Email)
                    .FirstOrDefault(), // Fetch the user's email
                a.StudyGroupId,
                Comments = a.Comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    CreatedBy = _context.Users
                        .Where(u => u.Id == c.CreatedByUserId)
                        .Select(u => u.Email)
                        .FirstOrDefault() // Fetch email for comment authors
                })
            })
            .ToListAsync();

        return Ok(announcements);
    }
}
