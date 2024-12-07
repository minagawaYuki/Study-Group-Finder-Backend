using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;
using StudyGroupFinder.Repository;

namespace StudyGroupFinder.Services
{
    public class SellerInfoService: ISellerInfoService
    {
        public readonly ISellerInfoRepository _sellerInfoRepository;

        public SellerInfoService(ISellerInfoRepository sellerInfoRepository)
        {
            _sellerInfoRepository = sellerInfoRepository;
        }
        public List<SellerDto> GetSellers()
        {
            List<SellerDto> result = new List<SellerDto>();

            var sellers = _sellerInfoRepository.GetSellers();

            foreach (Seller seller in sellers)
            {
                result.Add(new SellerDto()
                {
                    Name = seller.Name,
                    products = _sellerInfoRepository.GetProductBySellerId(seller.Id)
                });
            }

            return result;
        }
        public SellerDto GetSellerById(int id)
        {
            var seller = _sellerInfoRepository.GetSellerById(id);

            if (seller == null)
            {
                return null;
            }

            return
                new SellerDto()
                {
                    Name = seller.Name,
                    products = _sellerInfoRepository.GetProductBySellerId(id)
                };
        }
    }
}
