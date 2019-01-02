using Microsoft.AspNetCore.Mvc;
using QuEve.Web.Areas.Home.ViewModels;
using System.Diagnostics;

namespace QuEve.Web.Areas.Home.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Home")]
    [Route("")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Accesses the denied.
        /// </summary>
        /// <returns></returns>
        [Route("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Errors this instance.
        /// </summary>
        /// <returns></returns>
        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
