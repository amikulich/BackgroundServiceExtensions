using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyExtensions.BackgroundServiceExtensions
{
    public abstract class ScheduledJobServiceBase : BackgroundService
    {
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly ILogger _logger;

        protected ScheduledJobServiceBase(string cronExpression, ILogger logger)
        {
            _expression = CronExpression.Parse(cronExpression);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ScheduleJob(stoppingToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTime.UtcNow);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    try
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await ExecuteJobAsync(cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error occurred during a scheduled job execution");
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
