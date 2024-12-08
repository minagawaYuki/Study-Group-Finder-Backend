using Microsoft.EntityFrameworkCore;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;
using System.Threading.Tasks;

namespace StudyGroupFinder.Services
{
    public class StudyGroupService : IStudyGroupService
    {
        private readonly StudyGroupFinderDbContext _context;

        public StudyGroupService(StudyGroupFinderDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StudyGroupResponseDto>> GetAllStudyGroupsAsync()
        {
            return await _context.StudyGroups
                .Include(g => g.CreatedBy) // Include the user who created the group
                .Select(g => new StudyGroupResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Subject = g.Subject,
                    CourseCode = g.CourseCode,
                    MemberCount = g.Members.Count,
                    CreatedByFullName = g.CreatedBy.FullName,
                    CreatedByUserName = g.CreatedBy.UserName,
                    CreatedByEmail = g.CreatedBy.Email
                })
                .ToListAsync();
        }

        public async Task<StudyGroupResponseDto> GetGroupDetailsAsync(int groupId)
        {
            var group = await _context.StudyGroups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members) // If you have members data
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null) return null;

            return new StudyGroupResponseDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Subject = group.Subject,
                CourseCode = group.CourseCode,
                CreatedByEmail = group.CreatedBy?.Email,
                MemberCount = group.Members?.Count ?? 0
            };
        }




        public async Task<bool> CreateGroupAsync(StudyGroup studyGroup, string userId)
        {
            studyGroup.CreatedAt = DateTime.UtcNow;
            studyGroup.CreatedByUserId = userId;

            _context.StudyGroups.Add(studyGroup);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> JoinGroupAsync(int groupId, string userId)
        {
            // Check if the group exists
            var group = await _context.StudyGroups.FindAsync(groupId);
            if (group == null) return false;

            // Check if the user is already a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.StudyGroupId == groupId && gm.UserId == userId);
            if (isMember) return false;

            var groupMember = new GroupMember
            {
                StudyGroupId = groupId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
