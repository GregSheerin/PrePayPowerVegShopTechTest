using Microsoft.AspNetCore.Mvc;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("{csvData}")]
        public async Task<ActionResult<CheckOut>> CheckOutOrder([FromBody] string body)
        {
            //Rewind the body stream back, avoid a null string coming in
            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

            //Grab the stream of the csv out
            using var stream = new StreamReader(HttpContext.Request.Body);

            //parse the order
            var productOrder = await _csvParser.ParseCsv(stream).ConfigureAwait(false);


            if (productOrder == null || !productOrder.Any())
            {
                return BadRequest("No Product Data");
            }

            var checkOutReceipt = await _checkOutService.CheckOutProducts(productOrder).ConfigureAwait(false);

            return Ok(checkOutReceipt);
        }
    }
}
