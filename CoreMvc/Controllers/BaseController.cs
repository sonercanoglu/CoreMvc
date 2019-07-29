using CoreMvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMvc.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        public CoreMvcDbContext _context;
        public ILogger<T> _log;
        public BaseController(CoreMvcDbContext context, ILogger<T> log)
        {
            _context = context;
            _log = log;
        }

        public IActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}

