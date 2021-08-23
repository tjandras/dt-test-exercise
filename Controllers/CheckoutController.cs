using System.Collections.Generic;
using System.Threading.Tasks;
using digitalthinkers.Interfaces;
using digitalthinkers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // Microsoft.AspNetCore.Mvc.Versioning package can be used to versioning the API
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService checkoutService;
        private readonly ILogger<CheckoutController> logger;

        public CheckoutController(ICheckoutService checkoutService, ILogger<CheckoutController> logger)
        {
            this.checkoutService = checkoutService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Dictionary<string, int>>> PostAsync(CustomerCheckout checkoutModel)
        {
            var checkoutResult = await checkoutService.CheckoutUserAsync(checkoutModel);
            if (checkoutResult.IsValid)
            {
                return Ok(checkoutResult.Change);
            }

            return BadRequest(new { Result = "Checkout failed", Details = checkoutResult.ErrorMessage });
        }
    }
}