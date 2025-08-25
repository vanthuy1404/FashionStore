using FashionStore.Data;
using FashionStore.DTOs;
using FashionStore.Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly FStoreDbContext _context;
        public StatisticsController(FStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<StatisticsDto>> GetStatistics()
        {
            // Tổng doanh thu (đơn hoàn thành)
            var totalRevenue = await _context.Orders
                .SumAsync(o => (decimal?)o.total) ?? 0;

            // Tổng số đơn
            var totalOrders = await _context.Orders.CountAsync();

            // Tổng sản phẩm
            var totalProducts = await _context.Products.CountAsync();

            // Tổng user
            var totalUsers = await _context.Users.CountAsync();

            // Doanh thu theo tháng
            var revenueByMonth = await _context.Orders
                .GroupBy(o => new { o.created_at.Year, o.created_at.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => x.total)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var revenueByMonthDtos = revenueByMonth
                .Select(g => new RevenueByMonthDto
                {
                    month = $"{g.Month:D2}/{g.Year}",
                    revenue = g.Revenue
                })
                .ToList();

            // Top sản phẩm bán chạy
            var topProducts = await _context.OrderItems
                .GroupBy(oi => new { oi.product_id, oi.Product!.name })
                .Select(g => new TopProductDto
                {
                    product_id = g.Key.product_id,
                    name = g.Key.name,
                    total_quantity = g.Sum(x => x.quantity),
                    total_revenue = g.Sum(x => x.quantity * x.price)
                })
                .OrderByDescending(x => x.total_quantity)
                .Take(5)
                .ToListAsync();

            // Trả về DTO
            var result = new StatisticsDto
            {
                total_revenue = totalRevenue,
                total_orders = totalOrders,
                total_products = totalProducts,
                total_users = totalUsers,
                revenue_by_month = revenueByMonthDtos,
                top_products = topProducts
            };

            return Ok(result);
        }
    }
}
