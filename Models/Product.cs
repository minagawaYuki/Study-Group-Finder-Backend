using System.Text.Json.Serialization;

namespace StudyGroupFinder.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int SellerId { get; set; }
        [JsonIgnore]
        public Seller Seller { get; set; }
    }
}
