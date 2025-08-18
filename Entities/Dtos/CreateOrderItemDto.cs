using System.Text.Json.Serialization;

namespace FashionStore.Entities.Dtos
{
    public class CreateOrderItemDto
    {
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}
