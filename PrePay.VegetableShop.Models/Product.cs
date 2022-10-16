using System.ComponentModel.DataAnnotations;

namespace PrePay.VegetableShop.Models
{
    public class Product
    {
        [Key]
        public ProductEnum ProductName { get; set; }
        public float Price { get; set; }
    }
}
