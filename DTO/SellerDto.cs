using StudyGroupFinder.Models;
using System.ComponentModel.DataAnnotations;

namespace StudyGroupFinder.DTO
{
    public class SellerDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public List<Product> products {  get; set; } 
    }
}
