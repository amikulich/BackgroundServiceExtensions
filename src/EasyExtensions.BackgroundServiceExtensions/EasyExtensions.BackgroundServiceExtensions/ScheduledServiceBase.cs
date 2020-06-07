using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Timer = System.Timers.Timer;

namespace EasyExtensions.BackgroundServiceExtensions
{
    public abstract class ScheduledServiceBase : BackgroundService
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly CronExpression _expression;

        protected ScheduledServiceBase(string expression)
            : this(expression, NullLogger.Instance)
        {
        }

        protected ScheduledServiceBase(string expression, ILogger logger)
        {
            _logger = logger;
            _expression = CronExpression.Parse(expression);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            await ScheduleJob(stoppingToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTime.UtcNow);

            if (next.HasValue)
            {
                _logger.LogInformation($"Next execution of {GetType().FullName} at {next}");
                var delay = next.Value - DateTimeOffset.Now;

                _timer = new Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ExecuteJobAsync(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);
                    }
                };
                _timer.Start();
            }

            await Task.CompletedTask;
        }

        public abstract Task ExecuteJobAsync(CancellationToken cancellationToken);
    }
}
