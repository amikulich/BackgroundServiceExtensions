# BackgroundServiceExtensions
Step by step guide

### 1. Install nuget package 

```Install-Package EasyExtensions.BackgroundServiceExtensions```

### 2. Create a class which will be executed by schedule:
```
public class MyBackgroundService : ScheduledServiceBase
{
    private readonly IServiceProvider _serviceProvider;

    public MyBackgroundService(ScheduledServiceOptions<CacheService> options, IServiceProvider serviceProvider) : base(options.Expression)
    {
        _serviceProvider = serviceProvider;
    }

    public override Task ExecuteJobAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            // code to be executed by schedule
        }

        return Task.CompletedTask;
    }
}
```                
### 3. Register the class in the Startup:

    public void ConfigureServices(IServiceCollection services)
    {
        // other registrations

        services.AddScheduledService<MyBackgroundService>(Configuration);
    }

### 4. Add a section to appsettings.json:
```
  {    
    //other configuration

    "ScheduledServices": {
      "MyBackgroundService": {
        "Expression": "*/1 * * * *",
        "Enabled": true
      }
    }
  }
```

### 5. Change CRON expression to whatever you want to. 

If you are not familiar with CRON expression I can recommend [this tool](https://crontab.guru/#*/_*_*_*_*) to start with.
