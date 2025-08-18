using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("cart_items")]
    public class CartItem
    {
        public string id { get; set; }  
        public string user_id { get; set; }  // ID người dùng sở hữu giỏ hàng
        public string product_id { get; set; }  // ID sản phẩm trong giỏ hàng
        public int quantity { get; set; }  // Số lượng sản phẩm trong giỏ hàng
        public string? size { get; set; }  // Kích thước sản phẩm
        public string? color { get; set; }  // Màu sắc sản phẩm
        public DateTime created_at { get; set; } = DateTime.UtcNow;  // Ngày tạo mục giỏ hàng

        // Tham chiếu đến bảng users
        [ForeignKey("user_id")]
        public User? User { get; set; }

        // Tham chiếu đến bảng products
        [ForeignKey("product_id")]
        public Product? Product { get; set; }
    }
}
