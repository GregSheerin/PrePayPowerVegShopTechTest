using System.Collections.Generic;

namespace PrePay.VegetableShop.Models
{
    public class CheckOut
    {
        public float TotalPrice { get; set; }
        public List<Product> ProductsPurchased { get; set; }
    }
}
