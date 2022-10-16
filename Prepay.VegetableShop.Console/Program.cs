using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrePay.VegetableShop.Models;

namespace Prepay.VegetableShop.ConsoleWorkBench
{
    //A small console workbench to serve as an easy way to just call the api
    //It should be noted that I didnt apply and proper practices to this
    //For example, the URi should be grabbed out of some config.
    //The idea behind this app is just to show easy it is to set up setting the order and sending it,
    //once the consumer get it back it can be used for whatever logic(front/backend/business/auditing ect) they need
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var receipt = await SendRequest();

            Console.WriteLine("Here is your receipt for your order:\n");
            Console.WriteLine("Here are the product you ordered:");
            WriteProductsToConsole(receipt.PurchasedProducts);

            Console.WriteLine($"Your total comes to {receipt.FinalPrice}\n");
            Console.WriteLine("Offers Applied :");
            receipt.OffersApplied.ForEach(Console.WriteLine);

            Console.WriteLine("Here is your final list of products are Offers:\n");
            WriteProductsToConsole(receipt.AllProducts);
            Console.WriteLine("Thanks for you shopping at the pre pay vegetable shop");
            Console.ReadLine();

        }

        static async Task<OrderReceipt> SendRequest()
        {
            var path = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\\ProductOrder.Csv";

            using var streamReader = new StreamReader(path);

            var inputData = await streamReader.ReadToEndAsync();

            var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:5001/");
            try
            {
                var response = await client.PostAsync($"api/Products", new StringContent(inputData, Encoding.UTF8));

                return JsonConvert.DeserializeObject<OrderReceipt>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static void WriteProductsToConsole(List<ProductOrder> itemsToDisplay)
        {
            itemsToDisplay.ForEach(item =>
            {
                Console.WriteLine($"{Enum.GetName(typeof(ProductEnum), item.ProductName)} : {item.Quantity}");
            });
        }
    }
}
