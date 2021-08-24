using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExceptionController : ControllerBase
    {
        private readonly ILogger<ExceptionController> logger;

        public ExceptionController(ILogger<ExceptionController> logger)
        {
            this.logger = logger;
        }

        [Route("/errors")]
        public IActionResult HandleException()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            logger.LogError(ex.Error, ex.Error.Message);
            return Problem("Unexpected error occured.");
        }
    }
}