using System.ComponentModel.DataAnnotations;

namespace StudyGroupFinder.DTO
{
    public class StudyGroupDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; }
    }
}
