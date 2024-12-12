using System.Text.Json.Serialization;

namespace StudyGroupFinder.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public int StudyGroupId { get; set; } // Foreign key to StudyGroup
        [JsonIgnore]
        public StudyGroup StudyGroup { get; set; } // Navigation property
        public string Content { get; set; } // Announcement content
        public string CreatedByUserId { get; set; } // User who created the announcement
        public ApplicationUser CreatedBy { get; set; } // Navigation property
        public DateTime CreatedAt { get; set; } // Timestamp
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; } // Foreign key to Announcement
        public Announcement Announcement { get; set; } // Navigation property
        public string Content { get; set; } // Comment content
        public string CreatedByUserId { get; set; } // User who created the comment
        public ApplicationUser CreatedBy { get; set; } // Navigation property
        public DateTime CreatedAt { get; set; } // Timestamp
    }
}
