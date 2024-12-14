using Microsoft.AspNetCore.Identity;

namespace StudyGroupFinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public ICollection<GroupMember> GroupMemberships { get; set; } // Relationship with groups

    }
}
