using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using digitalthinkers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // Microsoft.AspNetCore.Mvc.Versioning package can be used to versioning the API
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> logger;
        private readonly ICurrencyService currencyService;

        public StockController(ILogger<StockController> logger, ICurrencyService currencyService)
        {
            this.logger = logger;
            this.currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<int, int>>> GetAsync()
        {
            Dictionary<int, int> currentStock = await currencyService.GetCurrentStockAsync();
            return Ok(currentStock);
        }

        [HttpPost]
        public async Task<ActionResult<Dictionary<int, int>>> PostAsync(Dictionary<int, int> moneyToLoad)
        {
            Dictionary<int, int> updatedStock = await currencyService.UpdateStockAsync(moneyToLoad);

            return Ok(updatedStock);
        }
    }
}
