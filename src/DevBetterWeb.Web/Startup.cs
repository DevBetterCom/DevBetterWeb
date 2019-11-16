using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.Identity.UI.Services;
using DevBetterWeb.Web.Services;

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

        public IServiceProvider ConfigureProductionServices(IServiceCollection services)
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
            return ConfigureServices(services);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/ArchivedVideos");
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddScoped<IRepository, EfRepository>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            return BuildDependencyInjectionProvider(services);
        }

        private static IServiceProvider BuildDependencyInjectionProvider(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            // Populate the container using the service collection
            builder.Populate(services);

            Assembly webAssembly = Assembly.GetExecutingAssembly();
            Assembly coreAssembly = Assembly.GetAssembly(typeof(BaseEntity));
            Assembly infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository)); // TODO: Move to Infrastucture Registry

            // consider replacing with AppDomain.CurrentDomain.GetAssemblies()
            Assembly[] assemblies = new Assembly[] { webAssembly, coreAssembly, infrastructureAssembly };

            builder.RegisterAssemblyTypes(assemblies).AsImplementedInterfaces();

            // register domain event handlers
            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(IHandle<>))
                .InstancePerLifetimeScope();

            // register container with itself so it can be passed to DomainEventDispatcher
            //IContainer applicationContainer = null;
            //builder.Register(c => applicationContainer).AsSelf();
            //applicationContainer = builder.Build();

            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
