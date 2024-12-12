namespace StudyGroupFinder.Models
{
    public class GroupMember
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to ApplicationUser
        public ApplicationUser User { get; set; } // Navigation property
        public int StudyGroupId { get; set; } // Foreign key to StudyGroup
        public StudyGroup StudyGroup { get; set; } // Navigation property
        public DateTime JoinedAt { get; set; }
    }
}
    