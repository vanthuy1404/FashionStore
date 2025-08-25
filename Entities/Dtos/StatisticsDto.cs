namespace FashionStore.DTOs
{
    public class StatisticsDto
    {
        public decimal total_revenue { get; set; }
        public int total_orders { get; set; }
        public int total_products { get; set; }
        public int total_users { get; set; }
        public List<RevenueByMonthDto> revenue_by_month { get; set; } = new();
        public List<TopProductDto> top_products { get; set; } = new();
    }

    public class RevenueByMonthDto
    {
        public string month { get; set; } = string.Empty; // "MM/yyyy"
        public decimal revenue { get; set; }
    }

    public class TopProductDto
    {
        public string product_id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int total_quantity { get; set; }
        public decimal total_revenue { get; set; }
    }
}
