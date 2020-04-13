using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Components
{
    public class BreadcrumbsWidget : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dictionary = new Dictionary<string, (string controller, string action)>
            {
                ["Home"] = (controller: "Home", action: "Index"),
                [$"{Request.RouteValues["controller"]}"] = (controller: $"{Request.RouteValues["controller"]}", action: "Index")
            };

            if ((string) Request.RouteValues["action"] != "Index")
            {
                dictionary[$"{Request.RouteValues["action"]}"] = (controller: $"{Request.RouteValues["controller"]}", action: $"{Request.RouteValues["action"]}");
            }

            return View(dictionary);
        }
    }
}
