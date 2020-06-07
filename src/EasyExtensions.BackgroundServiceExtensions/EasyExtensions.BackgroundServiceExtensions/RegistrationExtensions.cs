using System;
using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyExtensions.BackgroundServiceExtensions
{
    public static class RegistrationExtensions
    {
        private static readonly string ConfigPrefix = "ScheduledServices";

        /// <summary>
        /// Registers a scheduled service. By default reads configuration from ScheduledServices:{ServiceClassName} section
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <throws>ScheduledServiceRegistrationException</throws>
        /// <returns></returns>
        public static IServiceCollection AddScheduledService<T>(this IServiceCollection services,
            IConfiguration config) where T : ScheduledServiceBase
        {
            var sectionName = $"{ConfigPrefix}:{typeof(T).Name}";
            var options = ParseOptions<ScheduledServiceOptions<T>>(config, sectionName);

            if (options.Enabled)
            {
                EnsureCronExpressionValid(options.Expression);
                services.AddSingleton<ScheduledServiceOptions<T>>(options);
                services.AddHostedService<T>();
            }

            return services;
        }

        /// <summary>
        /// Registers a scheduled service together with its options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddScheduledService<T, TOptions>(this IServiceCollection services, TOptions options) 
            where T : ScheduledServiceBase where TOptions : ScheduledServiceOptions<T>
        {
            if (options.Enabled)
            {
                EnsureCronExpressionValid(options.Expression);
                services.AddSingleton(options);
                services.AddHostedService<T>();
            }

            return services;
        }

        private static void EnsureCronExpressionValid(string expression)
        {
            try
            {
                CronExpression.Parse(expression);
            }
            catch (CronFormatException e)
            {
                throw new ScheduledServiceRegistrationException(e.Message, e);
            }
        }

        private static TOptions ParseOptions<TOptions>(IConfiguration config, string sectionName)
        {
            var section = config.GetSection(sectionName);
            if (section == null)
            {
                throw new ScheduledServiceRegistrationException($"Missing scheduled service configuration. Verify a section with name {sectionName} exists");
            }

            return section.Get<TOptions>();
        }
    }
}
