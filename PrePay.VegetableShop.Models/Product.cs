using System.ComponentModel.DataAnnotations;

namespace PrePay.VegetableShop.Models
{
    public class Product
    {
        [Key]
        public ProductEnum ProductName { get; set; } //TODO : Rename this, not a name really
        public float Price { get; set; }
    }
}
