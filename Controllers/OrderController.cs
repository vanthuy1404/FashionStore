using FashionStore.Data;
using FashionStore.Entities.Dtos;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly FStoreDbContext _context;

        public OrderController(FStoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Order
       [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "Cart is empty" });

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Product {item.ProductId} not found" });

                var orderItem = new OrderItem
                {
                    id = Guid.NewGuid().ToString(),
                    product_id = item.ProductId,
                    quantity = item.Quantity,
                    size = item.Size,
                    color = item.Color,
                    price = product.price
                };

                total += product.price * item.Quantity;
                orderItems.Add(orderItem);
            }

            // ✅ Nếu có coupon
            Coupon? coupon = null;
            if (!string.IsNullOrEmpty(dto.CouponId))
            {
                coupon = await _context.Coupons.FindAsync(dto.CouponId);
                if (coupon == null)
                {
                    return BadRequest(new { message = "Invalid coupon" });
                }

                // kiểm tra thời gian hợp lệ
                if (coupon.ngay_bat_dau > DateTime.UtcNow || coupon.ngay_ket_thuc < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Coupon is expired or not yet valid" });
                }

                // áp dụng giảm giá
                total = total - (total * coupon.phan_tram / 100);
                
            }
            // tính phí ship
            if (dto.ShippingFee != null)
            {
                total = total + Convert.ToDecimal(dto.ShippingFee);
            }

            var order = new Order
            {
                id = Guid.NewGuid().ToString(),
                user_id = dto.UserId,
                address = dto.Address,
                phone = dto.Phone,
                total = total,
                coupon_id = coupon?.id,  // lưu coupon nếu có
                OrderItems = orderItems,
                shipping_fee = dto.ShippingFee ?? 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                orderId = order.id,
                total = order.total,
            });
        }


        // GET: api/Order/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrdersByUser(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.user_id == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Coupon) // ✅ include thêm coupon
                .OrderByDescending(o => o.created_at)
                .ToListAsync();

            var result = orders.Select(o => new OrderDTO
            {
                Id = o.id,
                Total = o.total,
                Status = o.status,
                Address = o.address,
                Phone = o.phone,
                Date = o.created_at,

                // ✅ Trả về thêm thông tin coupon
                MaCoupon = o.Coupon != null ? o.Coupon.ma_coupon : "Không áp dụng",
                PhanTram = o.Coupon != null ? o.Coupon.phan_tram : 0,
                ShippingFee = o.shipping_fee ?? 0,

                Items = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    Id = oi.id,
                    Size = oi.size,
                    Color = oi.color,
                    Quantity = oi.quantity,
                    Product = new ProductDTO
                    {
                        Id = oi.Product.id,
                        Name = oi.Product.name,
                        Price = oi.Product.price
                    }
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> GetAllOrder()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Coupon) // ✅ include thêm coupon
                .OrderByDescending(o => o.created_at)
                .ToListAsync();

            var result = orders.Select(o => new OrderDTO
            {
                Id = o.id,
                Total = o.total,
                Status = o.status,
                Address = o.address,
                Phone = o.phone,
                Date = o.created_at,
                // ✅ Trả về thêm thông tin coupon
                MaCoupon = o.Coupon != null ? o.Coupon.ma_coupon : "Không áp dụng",
                PhanTram = o.Coupon != null ? o.Coupon.phan_tram : 0,
                ShippingFee = o.shipping_fee ?? 0,
                Items = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    Id = oi.id,
                    Size = oi.size,
                    Color = oi.color,
                    Quantity = oi.quantity,
                    Product = new ProductDTO
                    {
                        Id = oi.Product.id,
                        Name = oi.Product.name,
                        Price = oi.Product.price
                    }
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // PUT: api/Order/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateOrderStatusDto request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng" });
            var currentStatus = order.status;
            if (request.Status.ToLower() == "yêu cầu hủy")
            {
                if (currentStatus.ToLower() != "chờ xác nhận" || currentStatus.ToLower() != "đã hủy")
                {
                    return BadRequest(new { message = "Chỉ có thể yêu cầu hủy ở trạng thái Chờ xác nhận" });
                }
            }

            order.status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", order });
        }
        
    }
}
