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
                return BadRequest("Cart is empty");

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found");

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

            var order = new Order
            {
                id = Guid.NewGuid().ToString(),
                user_id = dto.UserId,
                address = dto.Address,
                phone = dto.Phone,
                total = total,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.id });
        }

        // GET: api/Order/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrdersByUser(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.user_id == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
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

            order.status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", order });
        }
    }
}
