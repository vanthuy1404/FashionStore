using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("orders")]
    public class Order
    {
        public string id { get; set; }           // PK
        public string user_id { get; set; }
        public decimal total { get; set; }
        public string status { get; set; } = "Chờ xác nhận";
        public string address { get; set; }
        public string phone { get; set; }
        public string? coupon_id { get; set; }  // FK đến Coupon
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; }
        [ForeignKey("coupon_id")]
        public Coupon? Coupon { get; set; }  // Navigation property đến Coupon
    }
}
