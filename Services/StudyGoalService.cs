using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;
using StudyGroupFinder.Repository;

namespace StudyGroupFinder.Services
{
    public class StudyGoalService : IStudyGoalService
    {
        private readonly IStudyGoalRepository _studyGoalRepository;

        public StudyGoalService(IStudyGoalRepository studyGoalRepository)
        {
            _studyGoalRepository = studyGoalRepository;
        }

        public async Task<StudyGoal> CreateStudyGoalAsync(string userId, StudyGoalDto dto)
        {
            var studyGoal = new StudyGoal
            {
                UserId = userId,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            return await _studyGoalRepository.CreateStudyGoalAsync(studyGoal);
        }

        public async Task<IEnumerable<StudyGoal>> GetUserStudyGoalsAsync(string userId)
        {
            return await _studyGoalRepository.GetUserStudyGoalsAsync(userId);
        }

        public async Task<bool> DeleteStudyGoalAsync(int id, string userId)
        {
            return await _studyGoalRepository.DeleteStudyGoalAsync(id, userId);
        }

        public async Task<bool> UpdateStudyGoalStatusAsync(int id, string userId, bool isCompleted)
        {
            return await _studyGoalRepository.UpdateStudyGoalStatusAsync(id, userId, isCompleted);
        }
        public async Task<StudyGoal> GetStudyGoalByIdAsync(int id, string userId)
        {
            return await _studyGoalRepository.GetStudyGoalByIdAsync(id, userId);
        }
    }
}
