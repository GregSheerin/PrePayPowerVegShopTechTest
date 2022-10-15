using System.Collections.Generic;

namespace PrePay.VegetableShop.Models
{
    public class CheckOut
    {
        public float TotalPrice { get; set; }
        public IAsyncEnumerable<ProductOrder> ProductsPurchased { get; set; }

        public CheckOut(IAsyncEnumerable<ProductOrder> productsPurchased)
        {
            ProductsPurchased = productsPurchased;
        }
    }
}
