using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Domain.Services.CheckoutService
{
    public class CheckOutService : ICheckOutService
    {
        private readonly IProductService _productContext;

        public CheckOutService(IProductService productContext)
        {
            _productContext = productContext;
        }

        public async Task<OrderReceipt> CheckOutProducts(List<ProductOrder> products)
        {
            //First validate to see if each product is available
            //We can assume that the both set of products align(that is to say, all the products ordered as avaiable)
            //The was csv helper works is that it will throw a csvhelper exception if it fails to parse
            //This can happen either loading the data or receiving, but it is based on the enum.
            //If the parsers runs into something not defined in the project enum, exception will be throw
            //If ever the data system was to chance(IE a database rather than a csv to store the details), a lot more validation would be needed
            //But as it stands, I can really on the csv helper handling any unknown(I catch it in some middleware to return a 500)
            var availableProducts = await _productContext.GetProducts().ConfigureAwait(false);

            var orderReceipt = await ProcessCheckout(products, availableProducts);

            return orderReceipt;
        }

        private static async Task<OrderReceipt> ProcessCheckout(IEnumerable<ProductOrder> products, List<Product> availableProducts)
        {
            var checkOut = new CheckOut(products.ToAsyncEnumerable());
            //first get the total price before apply any deals
            //The checkout is what the user is buying, the receipt is the return object details what happened during the checkout
            //This helps keep separate what was paid for(and thus would be applicable to deals), vs what the user is actual got
            //That is to say, you would get more for what you paid for, but a receipt should have details of what you actual ordered.
            await checkOut.ProductsPurchased.ForEachAsync(product =>
            {
                checkOut.TotalPrice += product.Quantity *
                                       availableProducts.First(x => x.ProductName == product.ProductName)!.Price;
            });

            var orderReceipt = new OrderReceipt
            {
                PurchasedProducts = await checkOut.ProductsPurchased.ToListAsync() //Store the original list, so we can let the user know exactly what the paid for
            };

            //Apply all the deal info
            checkOut = await ApplyDealsToCheck(checkOut, availableProducts, orderReceipt);

            //Finally construct the receipt for the user
            orderReceipt.FinalPrice = checkOut.TotalPrice;
            orderReceipt.AllProducts = CombineOrdersAsync(await checkOut.ProductsPurchased.ToListAsync(), orderReceipt.AllProducts);
            return orderReceipt;
        }

        //Since we can assume items can be added due to a deal, need to combine both lists at the end
        //Idea is the enum acts as an idnex for a product, and a qunaity is attached to that
        //There, need to find the existing entery in order recpeit, as we cant know in advance what extras people are going to get
        //This is assuming if the refactor was done to the order system, i could stable this into the various section but this is far neater
        private static List<ProductOrder> CombineOrdersAsync(List<ProductOrder> checkOutItems, List<ProductOrder> orderItems)
        {
            checkOutItems.ForEach(item =>
            {
                var existingItem = orderItems.FirstOrDefault(ord => ord.ProductName == item.ProductName);
                if (existingItem == null)
                {
                    orderItems.Add(item);
                    return;
                }

                var checkoutItem = checkOutItems.FirstOrDefault(ord => ord.ProductName == item.ProductName);
                existingItem!.Quantity += checkoutItem!.Quantity;

            });
            return orderItems;
        }

        //Ideally I would refactor this into something a lot more elegant, a full refactor of this would take a lot of time
        //But my ideal simulation would be to either separate this out into another service(doesn't seem correct for this, as the deals as directly related to checking out)
        //Or to have a service that gets a collection of deals(following some parseable format, pull the the database, then apply then based on the order)
        //Note that the return have the orginal and extra products, I did see as it seems likely that consumer would like a receipt of what they order
        //Aswell as what they got and the final price, I was careful to make sure that any extras gained via deals, dont loop back on themselves
        //IE getting free aubergines add more discounts, this would also need to be accounted for in a cleaner system,See services in readme for more details
        private static async Task<CheckOut> ApplyDealsToCheck(CheckOut checkOut, List<Product> availableProducts, OrderReceipt orderReceipt)
        {
            //Once we got all the values, we can apply them. I am assuming that the free items given wont in turn lead to more deals
            //IE that the deals are based only on what is paid for
            if (await checkOut.ProductsPurchased.FirstOrDefaultAsync(order => order.ProductName == ProductEnum.Aubergine) != null)
            {
                checkOut = await ApplyAubergineDiscount(checkOut, availableProducts, orderReceipt);
            }

            if (await checkOut.ProductsPurchased.FirstOrDefaultAsync(order =>
                    order.ProductName == ProductEnum.Tomato) == null)
            {
                return checkOut;
            }

            orderReceipt = await ApplyTomatoDeal(checkOut, orderReceipt); //Add the extra onto the order and not the orginal list
            checkOut = await ApplyTomatoDiscount(checkOut, availableProducts, orderReceipt);

            return checkOut;
        }

        //Aubergine Rule, buy 3 minus cost of 1
        private static async Task<CheckOut> ApplyAubergineDiscount(CheckOut checkOut, IEnumerable<Product> availableProducts, OrderReceipt orderReceipt)
        {
            var aubergineOrder = await checkOut.ProductsPurchased.FirstAsync(x => x.ProductName == ProductEnum.Aubergine)
                .ConfigureAwait(false);

            // ReSharper disable once PossibleLossOfFraction : I am using the fraction loss here as a round down
            var totalDiscount = aubergineOrder.Quantity / 3 *
                                availableProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Aubergine)!.Price;

            checkOut.TotalPrice -= totalDiscount;
            orderReceipt.OffersApplied.Add($"You bought {aubergineOrder.Quantity} {nameof(ProductEnum.Aubergine)}, so you get a total of {totalDiscount} deducted from your total");
            return checkOut;
        }

        //Tomato deal, buy 2 get 1 aubergine
        private static async Task<OrderReceipt> ApplyTomatoDeal(CheckOut checkOut, OrderReceipt orderReceipt)
        {
            var tomatoOrder = await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Tomato)
                .ConfigureAwait(false);

            var extraAubergines = tomatoOrder!.Quantity / 2;

            if (extraAubergines <= 0)
            {
                return orderReceipt;
            }

            orderReceipt.OffersApplied.Add($"Because you bought {tomatoOrder.Quantity} {nameof(ProductEnum.Tomato)}, you get {extraAubergines} {nameof(ProductEnum.Aubergine)} for free!");

            if (orderReceipt.AllProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Aubergine) == null)
            {
                orderReceipt.AllProducts.Add(new ProductOrder { ProductName = ProductEnum.Aubergine, Quantity = extraAubergines });
                return orderReceipt;
            }

            orderReceipt.AllProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Aubergine)!.Quantity +=
                extraAubergines;

            return orderReceipt;
        }

        //Every 4 dollar spent on totmates, -1 dollar
        private static async Task<CheckOut> ApplyTomatoDiscount(CheckOut checkOut, IEnumerable<Product> availableProducts, OrderReceipt orderReceipt)
        {
            var tomatoOrder = await checkOut.ProductsPurchased.FirstOrDefaultAsync(x => x.ProductName == ProductEnum.Tomato);
            var tomatoTotal = tomatoOrder!.Quantity *
                              availableProducts.FirstOrDefault(x => x.ProductName == ProductEnum.Tomato)!.Price;
            var totalDiscount = (int)Math.Floor(tomatoTotal / 4);

            if (totalDiscount <= 0)
            {
                return checkOut;
            }

            orderReceipt.OffersApplied.Add($"Because you bought a total of {tomatoOrder.Quantity} in {nameof(ProductEnum.Tomato)}, you get {totalDiscount} deducted from your total!");
            checkOut.TotalPrice -= totalDiscount;

            return checkOut;
        }
    }
}
