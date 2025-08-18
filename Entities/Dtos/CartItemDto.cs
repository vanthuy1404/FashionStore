namespace FashionStore.Entities.Dtos
{
    public class CartItemDto
    {
        public string user_id { get; set; }
        public string product_id { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
        public string color { get; set; }
    }
}
