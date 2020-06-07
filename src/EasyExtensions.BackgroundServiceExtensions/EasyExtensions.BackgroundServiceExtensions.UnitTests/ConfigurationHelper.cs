using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace EasyExtensions.BackgroundServiceExtensions.UnitTests
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration(string body)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(body);
            using var stream = new MemoryStream(byteArray);
            return new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }
    }
}
