using System;

namespace EasyExtensions.BackgroundServiceExtensions
{
    public class ScheduledServiceRegistrationException : Exception
    {
        public ScheduledServiceRegistrationException(string message, Exception e) : base(message, e)
        {
        }

        public ScheduledServiceRegistrationException(string message) : base(message)
        {
        }
    }
}
