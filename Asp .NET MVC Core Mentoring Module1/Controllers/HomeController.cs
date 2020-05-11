using System.Diagnostics;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Asp_.NET_MVC_Core_Mentoring_Module1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
