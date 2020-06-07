using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyExtensions.BackgroundServiceExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Api.BackgroundServices;

namespace Sample.Api
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
            services.AddControllers();

            //configuration will be loaded from a section ScheduledServices:CacheService
            services.AddScheduledService<CacheService>(Configuration);


            var advancedServiceConfig = Configuration
                .GetSection($"ScheduledServices:{nameof(AdvancedCacheService)}")
                .Get<AdvancedCacheServiceOptions>();
            services.AddScheduledService<AdvancedCacheService, AdvancedCacheServiceOptions>(advancedServiceConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
