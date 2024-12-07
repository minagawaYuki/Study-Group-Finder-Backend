using Microsoft.AspNetCore.Mvc;
using StudyGroupFinder.DTO;
using StudyGroupFinder.Models;

namespace StudyGroupFinder.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController: ControllerBase
    {
        public ProductsController() { }

        [HttpGet]
        public IActionResult getProducts()
        {
            return Ok(TempDb.Products);
        }

        [HttpGet]
        [Route("{id}", Name = "getProductById")]
        public IActionResult getProductById(int id)
        {
            var product = TempDb.Products.SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound($"Product with id {id} does not exist.");
            }
            return Ok(product);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult deleteProductsById(int id)
        {
            var product = TempDb.Products.SingleOrDefault(x => x.Id == id);
            if (product != null)
            {
                TempDb.Products.Remove(product);
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult createProduct(ProductDto productDto)
        {
            int id = TempDb.Products.Max(x => x.Id) + 1;

            Product product = new Product()
            {
                Id = id,
                Name = productDto.Name,
                Price = productDto.Price,
            };
            TempDb.Products.Add(product);

            return CreatedAtRoute("GetProductById", new { id = id }, product);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto productDto)
        {
            var existingProduct = TempDb.Products.SingleOrDefault(x => x.Id == id);

            if (existingProduct == null)
            {
                return NotFound($"Product with id {id} does not exist.");
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Price = productDto.Price;
            existingProduct.SellerId = productDto.SellerID;
            return Ok(existingProduct);
        }
    }
}
