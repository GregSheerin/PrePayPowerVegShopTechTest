using System.Collections.Generic;

namespace PrePay.VegetableShop.Models
{
    public class OrderReceipt
    {
        public List<ProductOrder> PurchasedProducts { get; set; }
        public float FinalPrice { get; set; }
        public List<string> OffersApplied { get; set; }
        public List<ProductOrder> AllProducts { get; set; }

        public OrderReceipt()
        {
            //Init these here as I want to push to them as we go along
            AllProducts = new List<ProductOrder>();
            OffersApplied = new List<string>();
        }
    }
}
