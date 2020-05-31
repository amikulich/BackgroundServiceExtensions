namespace EasyExtensions.BackgroundServiceExtensions
{
    public class ScheduledJobOptions<T> where T : ScheduledJobServiceBase 
    {
        public string Expression { get; set; }

        public bool Enabled { get; set; }
    }
}
