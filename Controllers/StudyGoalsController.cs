using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Services;
using System.Security.Claims;

namespace StudyGroupFinder.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/studygoals")]
    public class StudyGoalsController : ControllerBase
    {
        private readonly IStudyGoalService _studyGoalService;

        public StudyGoalsController(IStudyGoalService studyGoalService)
        {
            _studyGoalService = studyGoalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyGoal([FromBody] StudyGoalDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studyGoal = await _studyGoalService.CreateStudyGoalAsync(userId, dto);
            return CreatedAtAction(nameof(GetStudyGoal), new { id = studyGoal.Id }, studyGoal);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserStudyGoals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studyGoals = await _studyGoalService.GetUserStudyGoalsAsync(userId);
            return Ok(studyGoals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudyGoal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studyGoal = await _studyGoalService.GetStudyGoalByIdAsync(id, userId);
            if (studyGoal == null)
            {
                return NotFound(new { Message = "Study goal not found." });
            }

            return Ok(studyGoal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyGoal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _studyGoalService.DeleteStudyGoalAsync(id, userId);
            if (!result) return NotFound(new { Message = "Study goal not found or unauthorized." });

            return Ok(new { Message = "Study goal deleted successfully." });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStudyGoalStatus(int id, [FromBody] bool isCompleted)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _studyGoalService.UpdateStudyGoalStatusAsync(id, userId, isCompleted);
            if (!result) return NotFound(new { Message = "Study goal not found or unauthorized." });

            return Ok(new { Message = "Study goal status updated successfully." });
        }
    }
}
