using Microsoft.EntityFrameworkCore;
using StudyGroupFinder.Models;

namespace StudyGroupFinder.Repository
{
    public class StudyGoalRepository : IStudyGoalRepository
    {
        private readonly StudyGroupFinderDbContext _context;

        public StudyGoalRepository(StudyGroupFinderDbContext context)
        {
            _context = context;
        }

        public async Task<StudyGoal> CreateStudyGoalAsync(StudyGoal studyGoal)
        {
            _context.StudyGoals.Add(studyGoal);
            await _context.SaveChangesAsync();
            return studyGoal;
        }

        public async Task<StudyGoal> GetStudyGoalByIdAsync(int id, string userId)
        {
            return await _context.StudyGoals
                .FirstOrDefaultAsync(sg => sg.Id == id && sg.UserId == userId);
        }

        public async Task<IEnumerable<StudyGoal>> GetUserStudyGoalsAsync(string userId)
        {
            return await _context.StudyGoals
                .Where(sg => sg.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteStudyGoalAsync(int id, string userId)
        {
            var studyGoal = await GetStudyGoalByIdAsync(id, userId);
            if (studyGoal == null) return false;

            _context.StudyGoals.Remove(studyGoal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStudyGoalStatusAsync(int id, string userId, bool isCompleted)
        {
            var studyGoal = await GetStudyGoalByIdAsync(id, userId);
            if (studyGoal == null) return false;

            studyGoal.IsCompleted = isCompleted;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
