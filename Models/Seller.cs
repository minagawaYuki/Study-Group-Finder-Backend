namespace StudyGroupFinder.Models
{
    public class Seller
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
