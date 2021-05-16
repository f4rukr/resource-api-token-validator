using ADB2C.Repository.EFCore.Implementations;
using ADB2C.Repository.EFCore.Interfaces;
using ADB2C.ResourceApi.Extensions.Startup;
using ADB2C.Service.OIDC;
using ADB2C.Service.OIDC.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ADB2C.ResourceApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));
            services.AddScoped<IOIDCService, OIDCService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddAuthentication(Configuration);
            services.AddControllers();
            services.AddSwaggerConfiguration();
            services.AddCors(options => { options.AddPolicy("CORS", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.ReportApiVersions = true;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseTokenValidation();
            app.UseCors("CORS");
            app.UseSwaggerConfiguration();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
