using StudyGroupFinder.Models;
    
namespace StudyGroupFinder
{
    public class TempDb
    {
        public static List<Product> Products = new List<Product>()
        {
            new Product()
            {
                Id = 1,
                Name = "Chicharon",
                Price = 123.32f,
                SellerId = 1,
            },
            new Product()
            {
                Id = 2,
                Name = "Adobo",
                Price = 123.32f,
                SellerId = 1,
            },
            new Product()
            {
                Id = 3  ,
                Name = "Ginaling",
                Price = 123.32f,
                SellerId = 2,
            },
        };

        public static List<Seller> Sellers = new List<Seller>()
        {
            new Seller() {
                Id = 1,
                Name = "Emall",
        },
            new Seller() {
                Id = 2,
                Name = "SM",
            }
        };
    }
}
