using System.ComponentModel.DataAnnotations;

namespace StudyGroupFinder.DTO
{
    public class ProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int SellerID { get; set; }
    }
}
