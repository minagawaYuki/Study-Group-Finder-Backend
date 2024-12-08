namespace StudyGroupFinder.Models
{
    public class StudySession
    {
        public int Id { get; set; }
        public int StudyGroupId { get; set; } // Foreign key to StudyGroup
        public StudyGroup StudyGroup { get; set; } // Navigation property
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime ScheduledAt { get; set; }
        public ICollection<SessionParticipant> Participants { get; set; }
    }
}
