using Examples.Models;
using FracturedCode.FracturedThrottle;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Examples.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IThrottleCache _throttleCache;

        public HomeController(ILogger<HomeController> logger, IThrottleCache throttleCache)
        {
            _logger = logger;
            _throttleCache = throttleCache;
        }

        [Throttle(Rate = 5, RateType = RateTypeEnum.Minute)]
        [Throttle(Rate = 10, RateType = RateTypeEnum.Hour)]
        public IActionResult Index()
        {
            return View();
        }

        [Throttle(Rate = 10, RateType = RateTypeEnum.Minute)]
        public IActionResult Privacy(bool blockMe = false)
        {
            // You cannot use the cacheModifier to access another action's cache.
            // If you want this functionality, please contact the maintainer.
            // It is designed to access additional/separate caches on the same action.
            const string successCache = "DifferentiatingString";
            if (_throttleCache.AllowConnection(new Throttle { Rate = 1, RateType = RateTypeEnum.Minute}, successCache))
            {
                if (blockMe)
                {
                    _throttleCache.CacheConnection(successCache);
                }
                return View();
            } else
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}