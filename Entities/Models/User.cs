using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("users")]
    public class User
    {
        public string id { get; set; }  // UUID lưu dạng string
        public string email { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
