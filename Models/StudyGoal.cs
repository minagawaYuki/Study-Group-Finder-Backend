namespace StudyGroupFinder.Models
{
    public class StudyGoal
    {
        public int Id { get; set; }
        public string UserId { get; set; } // The owner of the goal
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; } // Status of the goal (true = completed, false = not completed)
    }
}
