namespace FashionStore.Entities.Dtos
{
    public class OrderItemDTO
    {
        public string Id { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }

        public ProductDTO Product { get; set; }
    }

    public class ProductDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDTO
    {
        public string Id { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public string? MaCoupon { get; set; }
        public int? PhanTram { get; set; }

        public List<OrderItemDTO> Items { get; set; }
    }
}
