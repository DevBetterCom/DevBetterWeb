using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using DevBetterWeb.Infrastructure;
using Autofac;

namespace DevBetterWeb.Web
{
    public class Startup
    {
        private bool _isDbContextAdded = false;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration config,
            ILogger<Startup> logger)
        {
            Configuration = config;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            _logger.LogInformation("Configuring Production Services");
            if (!_isDbContextAdded)
            {
                _logger.LogInformation("Adding real sql server dbContext");

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration
                        .GetConnectionString("ProductionConnectionString")));
                _isDbContextAdded = true;
            }
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // TODO: Consider changing to check services collection for dbContext
            if (!_isDbContextAdded)
            {
                _logger.LogInformation("Adding in localDb dbContext");
                string dbName = Guid.NewGuid().ToString();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration
                        .GetConnectionString("DefaultConnectionString")));
                _isDbContextAdded = true;
            }

            services.AddMvc()
                .AddControllersAsServices()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/ArchivedVideos");
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddScoped<IRepository, EfRepository>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.InitializeAutofac(Assembly.GetExecutingAssembly());
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
