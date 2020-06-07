namespace EasyExtensions.BackgroundServiceExtensions
{
    public class ScheduledServiceOptions<T> where T : ScheduledServiceBase
    {
        public string Expression { get; set; }

        public bool Enabled { get; set; }
    }
}
