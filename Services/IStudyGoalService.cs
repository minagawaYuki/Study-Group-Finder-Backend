using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;

namespace StudyGroupFinder.Services
{
    public interface IStudyGoalService
    {
        Task<StudyGoal> CreateStudyGoalAsync(string userId, StudyGoalDto dto);
        Task<IEnumerable<StudyGoal>> GetUserStudyGoalsAsync(string userId);
        Task<StudyGoal> GetStudyGoalByIdAsync(int id, string userId);
        Task<bool> DeleteStudyGoalAsync(int id, string userId);
        Task<bool> UpdateStudyGoalStatusAsync(int id, string userId, bool isCompleted);
    }
}
