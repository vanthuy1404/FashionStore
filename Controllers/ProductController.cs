using FashionStore.Data;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly FStoreDbContext _context;
        public ProductController(FStoreDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product payload)
        {
            if (id != payload.id)
            {
                return BadRequest(new { message = "Id không khớp với payload" });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.id == id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }

            // cập nhật field từ payload
            product.name = payload.name;
            product.price = payload.price;
            product.image = payload.image;
            product.category = payload.category;
            product.sizes = payload.sizes;     // FE gửi dạng JSON string rồi
            product.colors = payload.colors;   // FE gửi dạng JSON string rồi
            product.description = payload.description;
            product.stock = payload.stock;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }
        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.id == id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa sản phẩm thành công" });
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product payload)
        {
            // tự tạo id nếu FE không gửi
            if (string.IsNullOrEmpty(payload.id))
            {
                payload.id = Guid.NewGuid().ToString();
            }

            payload.created_at = DateTime.UtcNow;

            await _context.Products.AddAsync(payload);
            await _context.SaveChangesAsync();

            return Ok(payload);
        }

    }
}
