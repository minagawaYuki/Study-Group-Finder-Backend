namespace StudyGroupFinder.Models
{
    public class SessionParticipant
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to ApplicationUser
        public ApplicationUser User { get; set; } // Navigation property
        public int StudySessionId { get; set; } // Foreign key to StudySession
        public StudySession StudySession { get; set; } // Navigation property
    }
}
