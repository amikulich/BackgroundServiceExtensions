using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyExtensions.BackgroundServiceExtensions
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection RegisterScheduledJob<T>(this IServiceCollection services,
            IConfiguration config) where T : ScheduledJobServiceBase
        {
            var section = config.GetSection($"ScheduledJobs:{typeof(T).Name}");
            if (section == null)
            {
                throw new InvalidOperationException($"Missing scheduled job configuration. Verify section ScheduledJobs:{typeof(T).Name} exists");
            }

            var options = section.Get<ScheduledJobOptions<T>>();

            if (options.Enabled)
            {
                services.AddSingleton<ScheduledJobOptions<T>>(options);
                services.AddHostedService<T>();
            }

            return services;
        }
    }
}
