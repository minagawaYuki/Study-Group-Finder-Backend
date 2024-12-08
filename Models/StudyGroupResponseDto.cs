namespace StudyGroupFinder.Models
{
    public class StudyGroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string CourseCode { get; set; }
        public int MemberCount { get; set; } // Optional: Number of members
        public string CreatedByFullName { get; set; } // Full name of the creator
        public string CreatedByUserName { get; set; } // Username of the creator
        public string CreatedByEmail { get; set; } // Email of the creator
    }
}
