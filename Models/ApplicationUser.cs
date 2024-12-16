using Microsoft.AspNetCore.Identity;

namespace StudyGroupFinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? YearLevel { get; set; }
        public string? Course { get; set; }
        public string? Description { get; set; }
        
        public ICollection<GroupMember> GroupMemberships { get; set; } // Relationship with groups

    }
}
