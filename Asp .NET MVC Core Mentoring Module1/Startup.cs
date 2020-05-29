using System.Threading.Tasks;
using Asp._NET_Core_Mentoring_Module1.Helpers;
using Asp_.NET_Core_Mentoring_Module1.Data;
using Asp_.NET_Core_Mentoring_Module1.Logging;
using Asp_.NET_MVC_Core_Mentoring_Module1.Helpers;
using Asp_.NET_MVC_Core_Mentoring_Module1.Middleware;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
            services.Configure<MvcOptions>(o => o.Filters.Add(new RequireHttpsAttribute()));

            SetUpDataServices(services);

            SetupHelperServices(services);

            SetUpServices(services);

            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200",
                            "https://localhost:44302");
                    });
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
                logging.AddFile("Logs/ts-{Date}.log", fileSizeLimitBytes: 1024 * 1024, minimumLevel: LogLevel.Trace);
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddOpenIdConnect(options =>
                {
                    options.Authority = "https://login.microsoftonline.com/" + Configuration["TenantId"];//tenant
                    options.ClientId = Configuration["ClientId"];//client;
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = "/auth/signin-callback";
                    options.SignedOutRedirectUri = "https://localhost:44302/";
                    options.TokenValidationParameters.NameClaimType = "name";
                })
                .AddCookie();
        }

        private static void SetUpServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IDiskImageCacheService), typeof(DiskImageCacheService));
            services.AddScoped(typeof(IEmailService), typeof(EmailService));
        }

        private static void SetupHelperServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IViewDataHelper), typeof(ViewDataHelper));
            services.AddScoped(typeof(IImageHelper), typeof(ImageHelper));
        }

        private void SetUpDataServices(IServiceCollection services)
        {
            services.AddDbContextPool<NorthWindContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("NorthWood"));
            });

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Identity"),
                    sqlOptions => { sqlOptions.MigrationsAssembly("Asp .NET MVC Core Mentoring"); });
            });

            services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context => await CustomErrorExceptionHandler(context));
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts(h => h.MaxAge(days: 365).IncludeSubdomains().Preload());
            app.UseCsp(options => options.DefaultSources(s => s.Self())
                .ImageSources(p => p.Self().CustomSources(configuration["ImageServerUrl"]))
                .StyleSources(p => p.Self().CustomSources("https://localhost:44302").UnsafeInlineSrc = true)
                .ScriptSources(p => p.Self().CustomSources("https://localhost:44302")));

            app.UseXfo(o =>
            {
                o.Deny();
                o.SameOrigin();
            });

            app.UseCors(_myAllowSpecificOrigins);

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
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
