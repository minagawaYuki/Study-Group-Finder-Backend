using StudyGroupFinder.Models;

namespace StudyGroupFinder.Repository
{
    public interface ISellerInfoRepository
    {
        public List<Seller> GetSellers();
        public Seller? GetSellerById(int id);
        public List<Product> GetProductBySellerId(int id);
    }
}
