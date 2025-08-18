namespace FashionStore.Entities.Models
{
    public class Order
    {
        public string id { get; set; }           // PK
        public string user_id { get; set; }
        public decimal total { get; set; }
        public string status { get; set; } = "Chờ xác nhận";
        public string address { get; set; }
        public string phone { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
