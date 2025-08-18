namespace FashionStore.Entities.Dtos
{
    public class CartItemGetDto
    {
        public string id { get; set; }
        public string user_id { get; set; }  // ID người dùng sở hữu giỏ hàng
        public string product_id { get; set; }  // ID sản phẩm trong giỏ hàng
        public string? product_name { get; set; }  // Tên sản phẩm
        public string? product_image { get; set; }  // Hình ảnh sản phẩm
        public decimal product_price { get; set; }  // Giá sản phẩm
        public decimal ? product_total { get; set; } 
        public int quantity { get; set; }  // Số lượng sản phẩm trong giỏ hàng
        public string? size { get; set; }  // Kích thước sản phẩm
        public string? color { get; set; }  // Màu sắc sản phẩm
        public DateTime created_at { get; set; }  // Ngày tạo mục giỏ hàng
    }
}
