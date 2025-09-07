using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Entities.Models
{
    [Table("coupon")]
    public class Coupon
    {
        [Key]
        [Column("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("ma_coupon")]
        public string ma_coupon { get; set; } = string.Empty;

        [Required]
        [Column("phan_tram")]
        public int phan_tram { get; set; }

        [Column("noi_dung")]
        public string noi_dung { get; set; } = string.Empty;

        [Required]
        [Column("ngay_bat_dau")]
        public DateTime ngay_bat_dau { get; set; }

        [Required]
        [Column("ngay_ket_thuc")]
        public DateTime ngay_ket_thuc { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
