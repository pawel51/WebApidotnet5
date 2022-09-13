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
            services.ConfigureCors();
            services.AddAutoMapper(typeof(Startup));

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
            
            
            services.AddHttpCacheHeaders();
            services.AddMemoryCache();
            
            services.AddHttpContextAccessor();
            services.AddAuthentication();
            

            // <-- Extensions -->
            // ******************
            // Integration with IIS
            services.ConfigureIISIntegration();
            // Logging
            services.ConfigureLoggerService();
            // Connection string & migrations
            services.ConfigureSqlContext(Configuration);
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
            services.ConfigureResponseCaching();
            // Response expiration time and validation
            services.ConfigureHttpCacheHeaders();

            // Rate Limiting (Throttling)
            services.ConfigureRateLimitingOptions();

            // Identification
            services.ConfigureIdentity();
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
            app.UseRouting();
            app.UseHttpCacheHeaders();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseIpRateLimiting();
        }

    }
}
