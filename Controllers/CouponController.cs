using FashionStore.Data;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly FStoreDbContext _context;

        public CouponController(FStoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Coupon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetAllCoupons()
        {
            var coupons = await _context.Coupons.ToListAsync();
            return Ok(coupons);
        }
        // GET: api/Coupon/valid
        [HttpGet("valid")]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetValidCoupons()
        {
            var now = DateTime.UtcNow; // hoặc DateTime.Now nếu bạn dùng giờ local
            var coupons = await _context.Coupons
                .Where(c => now >= c.ngay_bat_dau && now <= c.ngay_ket_thuc)
                .ToListAsync();

            return Ok(coupons);
        }

        // GET: api/Coupon/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(string id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound(new { message = "Coupon not found" });
            }
            return Ok(coupon);
        }

        // POST: api/Coupon
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] Coupon model)
        {
            model.id = Guid.NewGuid().ToString();
            model.created_at = DateTime.UtcNow;

            _context.Coupons.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCouponById), new { id = model.id }, model);
        }

        // PUT: api/Coupon/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(string id, [FromBody] Coupon model)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.id == id);
            if (coupon == null)
            {
                return NotFound(new { message = "Coupon not found" });
            }

            coupon.ma_coupon = model.ma_coupon;
            coupon.phan_tram = model.phan_tram;
            coupon.noi_dung = model.noi_dung;
            coupon.ngay_bat_dau = model.ngay_bat_dau;
            coupon.ngay_ket_thuc = model.ngay_ket_thuc;

            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();

            return Ok(coupon);
        }

        // DELETE: api/Coupon/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(string id)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.id == id);
            if (coupon == null)
            {
                return NotFound(new { message = "Coupon not found" });
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Coupon deleted successfully" });
        }
    }
}
