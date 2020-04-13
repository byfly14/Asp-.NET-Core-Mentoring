using System.IO;
using System.Threading.Tasks;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Http;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Middleware
{
    public class ImageCacheMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        public ImageCacheMiddleware(RequestDelegate next)
        {
            _nextDelegate = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;

            try
            {
                await using (var ms = new MemoryStream())
                {
                    context.Response.Body = ms;

                    await _nextDelegate(context);

                    if (context.Response.ContentType == "image/bmp")
                    {
                        var diskImageCacheService =
                            (IDiskImageCacheService)context.RequestServices.GetService(typeof(IDiskImageCacheService));

                        var bytes = ms.ToArray();

                        int.TryParse(context.Request.RouteValues["id"].ToString(), out var id);
                        diskImageCacheService.CacheImage(id, bytes);
                    }
                    
                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(originalBody);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
