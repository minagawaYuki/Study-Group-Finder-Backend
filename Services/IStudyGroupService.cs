using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;

namespace StudyGroupFinder.Services
{
    public interface IStudyGroupService
    {
        Task<IEnumerable<StudyGroupResponseDto>> GetAllStudyGroupsAsync();
        Task<StudyGroupResponseDto> GetGroupDetailsAsync(int groupId);
        Task<bool> CreateGroupAsync(StudyGroup studyGroup, string userId);
        Task<bool> JoinGroupAsync(int groupId, string userId);
    }
}
