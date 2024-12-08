using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudyGroupFinder.Models
{
    public class StudyGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Subject { get; set; }

        public string CourseCode { get; set; }
        public DateTime CreatedAt { get; set; }

        // These fields should not be required in the client payload
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    }

}
