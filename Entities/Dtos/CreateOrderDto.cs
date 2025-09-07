using System.Text.Json.Serialization;

namespace FashionStore.Entities.Dtos
{
    public class CreateOrderDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("coupon_id")]
        public string? CouponId { get; set; }  // Thêm trường coupon_id

        [JsonPropertyName("items")]
        public List<CreateOrderItemDto> Items { get; set; }
        
    }
}
