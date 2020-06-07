using System;
using System.Threading;
using System.Threading.Tasks;
using EasyExtensions.BackgroundServiceExtensions;

namespace Sample.Api.BackgroundServices
{
    public class AdvancedCacheService : ScheduledServiceBase
    {
        private readonly string _customData;

        public AdvancedCacheService(AdvancedCacheServiceOptions options) 
            : base(options.Expression) 
          //: base(options.Expression, someLogger) - you may inject logger to the base service
        {
            _customData = options.CustomData;
        }

        public override Task ExecuteJobAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"--- {nameof(AdvancedCacheService)} started ---");

            Console.WriteLine($"This is a custom parameter: {_customData}");
            Thread.Sleep(TimeSpan.FromSeconds(5));

            Console.WriteLine($"--- {nameof(AdvancedCacheService)} finished ---");

            return Task.CompletedTask;
        }
    }

    public class AdvancedCacheServiceOptions : ScheduledServiceOptions<AdvancedCacheService>
    {
        public string CustomData { get; set; }
    }
}
