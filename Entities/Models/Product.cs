using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("products")]
    public class Product
    {
        public string id { get; set; }  // UUID lưu dạng string
        public string name { get; set; }
        public decimal price { get; set; }
        public string? image { get; set; }
        public string category { get; set; }
        public string sizes { get; set; }   // JSON dạng string
        public string colors { get; set; }  // JSON dạng string
        public string? description { get; set; }
        public int stock { get; set; }
        public DateTime created_at { get; set; }
    }
}
