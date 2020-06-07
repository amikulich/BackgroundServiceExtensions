using System;
using System.Threading;
using System.Threading.Tasks;
using EasyExtensions.BackgroundServiceExtensions;

namespace Sample.Api.BackgroundServices
{
    public class CacheService : ScheduledServiceBase
    {
        public CacheService(ScheduledServiceOptions<CacheService> options) : base(options.Expression)
        {
        }

        public override Task ExecuteJobAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"--- {nameof(CacheService)} started ---");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            Console.WriteLine($"--- {nameof(CacheService)} finished ---");

            return Task.CompletedTask;
        }
    }
}
