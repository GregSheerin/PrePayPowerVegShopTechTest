using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ICheckOutService _checkOutService;
        private readonly ICsvParser<ProductOrder> _csvParser;

        public ProductsController(ICheckOutService checkOutService, ICsvParser<ProductOrder> csvParser)
        {
            _checkOutService = checkOutService;
            _csvParser = csvParser;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<CheckOut>> CheckOutOrder()
        {
            //Rewind the body stream back, avoid a null string coming in
            //Enabled via the Enablebuffing call in the startup
            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

            //Grab the stream of the csv out, 
            using var stream = new StreamReader(HttpContext.Request.Body);

            //parse the order
            var productOrder = await _csvParser.ParseCsv(stream).ConfigureAwait(false);

            //Before we do any processing of the order, make sure there is something to process first
            if (productOrder == null || !productOrder.Any())
            {
                return BadRequest("No Product Data");
            }

            //Call the checkout service to generate a order receipt to be send back to the user
            //Then the caller and display the data however they want(see console workbench project for a rough example)
            var checkOutReceipt = await _checkOutService.CheckOutProducts(productOrder).ConfigureAwait(false);

            //return the result, the assigment didnt specify any return type, so I am just using json here for simpilicies sake
            return new JsonResult(checkOutReceipt);
        }

    }
}
