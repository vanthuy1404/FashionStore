using FashionStore.Data;
using FashionStore.Entities.Dtos;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(FStoreDbContext context) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<CartItemGetDto>>> GetCart(string userId)
        {
            var cartItems = await context.CartItems
                .Where(c => c.user_id == userId)
                .Include(c => c.Product) // join sang bảng product
                .Select(c => new CartItemGetDto
                {
                    id = c.id,
                    user_id = c.user_id,
                    product_id = c.product_id,
                    product_name = c.Product != null ? c.Product.name : null,
                    product_image = c.Product != null ? c.Product.image : null,
                    product_price = c.Product != null ? c.Product.price : 0,
                    product_total = (c.Product != null ? c.Product.price : 0) * c.quantity,
                    quantity = c.quantity,
                    size = c.size,
                    color = c.color,
                    created_at = c.created_at
                })
                .ToListAsync();

            if (cartItems == null || cartItems.Count == 0)
            {
                return NotFound(new { message = "Giỏ hàng trống hoặc không tồn tại" });
            }

            return Ok(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.user_id) || string.IsNullOrEmpty(dto.product_id))
                return BadRequest("Thông tin không hợp lệ.");

            // kiểm tra item đã tồn tại chưa
            var existingItem = await context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.user_id == dto.user_id &&
                    c.product_id == dto.product_id &&
                    c.size == dto.size &&
                    c.color == dto.color);

            if (existingItem != null)
            {
                // đã tồn tại => tăng quantity
                existingItem.quantity += dto.quantity;
                context.CartItems.Update(existingItem);
            }
            else
            {
                // thêm mới
                var newItem = new CartItem
                {
                    id = Guid.NewGuid().ToString(),
                    user_id = dto.user_id,
                    product_id = dto.product_id,
                    quantity = dto.quantity,
                    size = dto.size,
                    color = dto.color
                };
                await context.CartItems.AddAsync(newItem);
            }

            await context.SaveChangesAsync();
            return Ok("Thêm vào giỏ hàng thành công!");
        }
        // API cập nhật số lượng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuantity(string id, [FromBody] UpdateCartDto dto)
        {
            var cartItem = await context.CartItems.FirstOrDefaultAsync(c => c.id == id);
            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            if (dto.quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than 0" });

            cartItem.quantity = dto.quantity;

            context.CartItems.Update(cartItem);
            await context.SaveChangesAsync();

            return Ok(new { message = "Quantity updated successfully" });
        }

        // API xóa cart item
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            var cartItem = await context.CartItems.FirstOrDefaultAsync(c => c.id == id);
            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            context.CartItems.Remove(cartItem);
            await context.SaveChangesAsync();
            return Ok(new { message = "Cart item deleted successfully" });
        }
    }
}
