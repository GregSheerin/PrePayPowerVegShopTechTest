using Microsoft.AspNetCore.Mvc;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrePay.Vegetable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ICheckOutService _checkOutService;

        public ProductsController(ICheckOutService checkOutService)
        {
            _checkOutService = checkOutService;
        }

        [HttpPost()]
        public async Task<ActionResult<CheckOut>> CheckOutOrder(List<Product> products) //TODO: Add parsers for csv to handle a list of products
        {
            if (!products.Any())
            {
                return BadRequest("Error:No Products to checkout"); //TODO : Better error handling,middleware might be an idea for this kinda of project layout
            }

            var checkOutRecipt = await _checkOutService.CheckOutProducts(products);
            return Ok(new Product()); //TODO : Implement Get By ID
        }
    }
}
