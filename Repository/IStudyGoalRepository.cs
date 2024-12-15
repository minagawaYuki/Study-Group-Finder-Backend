using StudyGroupFinder.Models;

namespace StudyGroupFinder.Repository
{
    public interface IStudyGoalRepository
    {
        Task<StudyGoal> CreateStudyGoalAsync(StudyGoal studyGoal);
        Task<StudyGoal> GetStudyGoalByIdAsync(int id, string userId);
        Task<IEnumerable<StudyGoal>> GetUserStudyGoalsAsync(string userId);
        Task<bool> DeleteStudyGoalAsync(int id, string userId);
        Task<bool> UpdateStudyGoalStatusAsync(int id, string userId, bool isCompleted);
    }
}
