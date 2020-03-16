using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity.UI.Services;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Infrastructure;
using Autofac;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Linq;

namespace DevBetterWeb.Web
{
    public class Startup
    {
        private bool _isDbContextAdded = false;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            Configuration = config;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope? AutofacContainer { get; private set; }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            if (!_isDbContextAdded)
            {

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration
                        .GetConnectionString("DefaultConnection")));
                _isDbContextAdded = true;
            }
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));

            // TODO: Consider changing to check services collection for dbContext
            if (!_isDbContextAdded)
            {
                string dbName = Guid.NewGuid().ToString();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration
                        .GetConnectionString("DefaultConnection")));
                _isDbContextAdded = true;
            }

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddMvc()
                .AddControllersAsServices()
                .AddRazorRuntimeCompilation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddScoped<IRepository, EfRepository>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.InitializeAutofac(Assembly.GetExecutingAssembly());
            if (_env.EnvironmentName == "Development")
            {
                // last registration wins
                // builder.RegisterType<NoOpEmailService>().As<IEmailService>();
            }

        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            AppDbContext migrationContext)
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
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

            // run migrations automatically on startup
            //migrationContext.Database.Migrate();
            //migrationContext = null;

        }
    }
}
