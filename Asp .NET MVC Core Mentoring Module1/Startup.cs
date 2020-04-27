using System.Threading.Tasks;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Asp_.NET_MVC_Core_Mentoring_Module1.Helpers;
using Asp_.NET_MVC_Core_Mentoring_Module1.Middleware;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Asp_.NET_MVC_Core_Mentoring_Module1
{
    public class Startup
    {
        readonly string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<NorthWindContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("NorthWood"));
                });

            services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
            services.AddScoped( typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IDiskImageCacheService), typeof(DiskImageCacheService));
            services.AddScoped(typeof(IViewDataHelper), typeof(ViewDataHelper));

            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200",
                            "http://localhost:8088");
                    });
            });



            services.AddControllersWithViews();

            services.AddMvc();

            services.AddMvcCore().AddApiExplorer();

            //swagger/v1/swagger.json
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.CustomOperationIds(description => (description.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
            });

            services.AddScoped<LoggingActionFilter>();
            services.AddLogging(logging =>
            {
                logging.AddFile("Logs/ts-{Date}.log", fileSizeLimitBytes: 1024 * 1024, minimumLevel:LogLevel.Trace);
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(  async context => await CustomErrorExceptionHandler(context));
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(_myAllowSpecificOrigins);

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseMiddleware<ImageCacheMiddleware>();
            app.UseSwagger(o => o.SerializeAsV2 = true);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "images",
                    pattern: "images/{id:int}",
                    defaults: new { controller = "Categories", action = "GetCategoryImage" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task CustomErrorExceptionHandler(HttpContext context)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";

            await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
            await context.Response.WriteAsync("<h1 class=\"text-danger\">Error</h1>\r\n");

            var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;

            var errorMessage = $"<h2 class=\"text-danger\">{exception.GetType().Name}" +
                               " was thrown! For more details see log file</h2>";

            await context.Response.WriteAsync($"{errorMessage}<br><br>\r\n");

            await context.Response.WriteAsync("<a href=\"/\">Back to main page</a><br>\r\n");
            await context.Response.WriteAsync("</body></html>\r\n");
            await context.Response.WriteAsync(new string(' ', 512));
        }
    }
}
