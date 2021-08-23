using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // Microsoft.AspNetCore.Mvc.Versioning package can be used to versioning the API
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;

        public StockController(ILogger<StockController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<Dictionary<int, int>> GetAsync()
        {
            return Ok(new Dictionary<int, int> {
                { 1000, 5},
                { 10000, 3},
                { 100, 13},
                { 10, 5},
            });
        }

        [HttpPost]
        public ActionResult<Dictionary<int, int>> PostAsync(Dictionary<int, int> moneyToLoad)
        {
            return Ok(new Dictionary<int, int> {
                { 1000, 5},
                { 10000, 3},
                { 100, 13},
                { 10, 5},
            });
        }
    }
}
