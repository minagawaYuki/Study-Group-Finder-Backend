using System.ComponentModel.DataAnnotations;

namespace StudyGroupFinder.DTO
{
    public class UpdateAnnouncementDto
    {
        [Required]
        public string Content { get; set; }
    }
}
