using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PrePay.VegetableShop.Data.UnitOfWork;
using PrePay.VegetableShop.Models;


namespace PrePay.VegetableShop.Domain.Services.CheckoutService
{
    public class CheckOutService : ICheckOutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckOut> CheckOutProducts(List<Product> products)
        {
            foreach (var product in products)
            {
                if (await _unitOfWork.Products.GetProduct(product.Name) == null)
                {
                    //TODO: Implement some form of better expection handling for the api
                    throw new Exception("TODO: Better exception for invalid product in list");
                }
            }

            //TODO : Parse products and return checkout
            var checkout = new CheckOut();
            return checkout;
        }
    }
}
