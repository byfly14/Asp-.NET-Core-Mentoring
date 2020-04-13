using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Helpers
{
    public static class DownloadImageHtmlHelper
    {
        public static HtmlString DownloadImage(this IHtmlHelper html, int id)
        {
            return new HtmlString($"<a href = \"/Categories/GetCategoryImage/{id}\"> Download Category Image using Html Helper</a>");
        }
    }
}
