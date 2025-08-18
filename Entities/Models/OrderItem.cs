using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("order_items")]
    public class OrderItem
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string product_id { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public decimal price { get; set; }
        [ForeignKey("order_id")]

        public Order Order { get; set; }
        [ForeignKey("product_id")]

        public Product Product { get; set; }

    }
}
