using StudyGroupFinder.Models;

namespace StudyGroupFinder.Repository
{
    public class SellerInfoRepository: ISellerInfoRepository
    {
        private readonly StudyGroupFinderDbContext _studyGroupFinderDbContext;

        public SellerInfoRepository(StudyGroupFinderDbContext studyGroupFinderDbContext)
        {
            _studyGroupFinderDbContext = studyGroupFinderDbContext;
        }
        public List<Seller> GetSellers()
        {
            return _studyGroupFinderDbContext.Sellers.ToList();
        }
        public Seller? GetSellerById(int id)
        {
            return _studyGroupFinderDbContext.Sellers.SingleOrDefault(x => x.Id == id);
        }
        public List<Product> GetProductBySellerId(int id)
        {
            return _studyGroupFinderDbContext.Products.Where(x => x.SellerId == id).ToList();
        }
    }
}
