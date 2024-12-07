using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;

namespace StudyGroupFinder.Services
{
    public interface ISellerInfoService
    {
        public List<SellerDto> GetSellers();
        public SellerDto? GetSellerById(int id);

    }
}
