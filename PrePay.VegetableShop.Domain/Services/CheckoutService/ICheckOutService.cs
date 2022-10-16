﻿using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Domain.Services.CheckoutService
{
    public interface ICheckOutService
    {
        public Task<OrderReceipt> CheckOutProducts(List<ProductOrder> products);
    }
}
