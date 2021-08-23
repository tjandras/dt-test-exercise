using System.Collections.Generic;
using digitalthinkers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // Microsoft.AspNetCore.Mvc.Versioning package can be used to versioning the API
    public class CheckoutController : ControllerBase
    {
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ILogger<CheckoutController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<Dictionary<string, int>> PostAsync(CustomerCheckout checkoutModel)
        {
            return Ok();
        }
    }
}