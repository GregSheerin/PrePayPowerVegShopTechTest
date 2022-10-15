using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PrePay.VegetableShop.Models;
using PrePay.VegetableShop.Domain.Services.ProductService;

namespace PrePay.VegetableShop.Domain.Services.CheckoutService
{
    public class CheckOutService : ICheckOutService
    {
        private readonly IProductService _productContext;

        public CheckOutService(IProductService productContext)
        {
            _productContext = productContext;
        }

        public async Task<CheckOut> CheckOutProducts(List<ProductOrder> products)
        {
            //First validate to see if each product is available
            var availableProducts = await _productContext.GetProducts().ConfigureAwait(false);

            var checkout = await CreateCheckout(products, availableProducts);

            return checkout;
        }

        private static async Task<CheckOut> CreateCheckout(IEnumerable<ProductOrder> products, List<Product> availableProducts)
        {
            var checkOut = new CheckOut(products.ToAsyncEnumerable());
            await checkOut.ProductsPurchased.ForEachAsync(product =>
            {
                checkOut.TotalPrice += product.Quantity *
                                       availableProducts.FirstOrDefault(x => x.ProductName == product.ProductName)!.Price;
            });

            return await ApplyDealsToCheck(checkOut, availableProducts);
        }

        //TODO : Refactor these into a something neater, apply a pattern? just want to get the checkout logic returning to the user
        private static async Task<CheckOut> ApplyDealsToCheck(CheckOut checkOut, List<Product> availableProducts)
        {
            //Once we got all the values, we can apply them. I am assuming that the free items given wont in turn lead to more deals
            //IE that the deals are based only on what is paid for
            if (await checkOut.ProductsPurchased.FirstOrDefaultAsync(order => order.ProductName == ProductEnum.Aubergine) != null)
            {
                checkOut = await ApplyAubergineDiscount(checkOut, availableProducts);
            }

            //Not ideal, adding new products will bloat this block over time, ideally the deals are linked to the types themselfs(need to think of a pattern)
            // ReSharper disable once InvertIf
            if (await checkOut.ProductsPurchased.FirstOrDefaultAsync(order => order.ProductName == ProductEnum.Tomato) != null)
            {
                checkOut = await ApplyTomatoDeal(checkOut);
                checkOut = await ApplyTomatoDiscount(checkOut, availableProducts);
            }

            return checkOut;
        }

        //Aubergine Rule, buy 3 minus cost of 1
        private static async Task<CheckOut> ApplyAubergineDiscount(CheckOut checkOut, IEnumerable<Product> availableProducts)
        {
            var aubergineOrder = await checkOut.ProductsPurchased.FirstAsync(x => x.ProductName == ProductEnum.Aubergine)
                .ConfigureAwait(false);

            // ReSharper disable once PossibleLossOfFraction : I am using the fraction loss here as a round down
            var totalDiscount = aubergineOrder.Quantity / 3 *
                                availableProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Aubergine)!.Price;

            checkOut.TotalPrice -= totalDiscount;
            return checkOut;
        }

        //Tomato deal, buy 2 get 1 aubergine
        private static async Task<CheckOut> ApplyTomatoDeal(CheckOut checkOut)
        {
            var tomatoOrder = await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Tomato)
                .ConfigureAwait(false);

            var extraAubergines = tomatoOrder!.Quantity / 2;

            if (extraAubergines <= 0) return checkOut;

            if (await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Aubergine)
                    .ConfigureAwait(false) == null)
            {
                var productsOrder = await checkOut.ProductsPurchased.ToListAsync();
                productsOrder.Add(new ProductOrder { ProductName = ProductEnum.Aubergine, Quantity = extraAubergines });
                checkOut.ProductsPurchased = productsOrder.ToAsyncEnumerable();
                return checkOut;
            }

            (await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Aubergine)
                    .ConfigureAwait(false))!.Quantity +=
                tomatoOrder!.Quantity / 2;
            return checkOut;
        }

        //Every 4 dollar spent on totmates, -1 dollar
        private static async Task<CheckOut> ApplyTomatoDiscount(CheckOut checkOut, IEnumerable<Product> availableProducts)
        {
            var tomatoOrder = await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Tomato);
            var totalDiscount = (int)Math.Floor(tomatoOrder!.Quantity *
                availableProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Tomato)!.Price / 4);

            checkOut.TotalPrice -= totalDiscount;
            return checkOut;
        }
    }
}
