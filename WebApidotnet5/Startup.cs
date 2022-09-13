using AspNetCoreRateLimit;
using Contracts;
using Entities.DTO.OutDto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Repository.DataShaping;
using System.IO;
using WebApidotnet5.ActionFilters;
using WebApidotnet5.Extensions;
using WebApidotnet5.Filters;
using WebApidotnet5.ServiceExtensions;
using WebApidotnet5.Utility;

namespace WebApidotnet5
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "\\nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // CORS Policy
            services.ConfigureCors();
            // Integration with IIS
            services.ConfigureIISIntegration();
            // Logging
            services.ConfigureLoggerService();
            // Connection string & migrations
            services.ConfigureSqlContext(Configuration);
            // Auto mapping nuget
            services.AddAutoMapper(typeof(Startup));
            // Trash Manager 
            services.ConfigureRepositoryManager();
            // Controller config (filters, caches, accept header)
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.Filters.Add(new GlobalFilterExample());
                config.CacheProfiles.Add("120SecondDuration", new CacheProfile { Duration = 120 });
            })  // MvcBuidler
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters()
                .AddCustomCSVFormatter()
                ;
            // HateToAs 
            services.AddCustomMediaTypes();
            // Versioning
            services.ConfigureVersioning();
            // Caching
            // Response caching
            services.AddHttpCacheHeaders();
            services.ConfigureResponseCaching();
            // Response expiration time and validation
            services.ConfigureHttpCacheHeaders();
            // Rate Limiting (Throttling)
            services.AddMemoryCache();
            services.ConfigureRateLimitingOptions();
            services.AddHttpContextAccessor();

            // Identification
            services.AddAuthentication();
            services.ConfigureIdentity();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddScoped<ActionFilterExample>();
            services.AddScoped<ControllerFilterExample>();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateCompanyExistsAttribute>();
            services.AddScoped<ValidateEmployeeForCompanyExistsAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
            services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
            services.AddScoped<EmployeeLinks>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseResponseCaching();
            app.UseIpRateLimiting();
            app.UseRouting();
            app.UseHttpCacheHeaders();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }

    }
}
