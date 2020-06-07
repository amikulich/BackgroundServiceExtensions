using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EasyExtensions.BackgroundServiceExtensions.UnitTests
{
    [TestFixture]
    public class RegistrationExtensionsTests
    {
        IServiceCollection _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ServiceCollection();
        }

        [Test]
        public void AddScheduledService_IncorrectCronExpression_ThrowsException()
        {
            var config = ConfigurationHelper.GetConfiguration("{\"ScheduledServices\":{\"BackgroundServiceStub\":{\"Enabled\":true,\"Expression\":\"wrong format\"}}}");

            var exception = Assert.Throws<ScheduledServiceRegistrationException>(() => _sut.AddScheduledService<BackgroundServiceStub>(config));
            Assert.IsInstanceOf<CronFormatException>(exception.InnerException);
        }

        [Test]
        public void AddScheduledService_ServiceDisabled_DoesnotAddToServiceCollection()
        {
            var config = ConfigurationHelper.GetConfiguration("{\"ScheduledServices\":{\"BackgroundServiceStub\":{\"Enabled\":false,\"Expression\":\"* * * * *\"}}}");

            _sut.AddScheduledService<BackgroundServiceStub>(config);
            CollectionAssert.IsEmpty(_sut);
        }

        [Test]
        public void AddScheduledService_HappyPath()
        {
            var config = ConfigurationHelper.GetConfiguration("{\"ScheduledServices\":{\"BackgroundServiceStub\":{\"Enabled\":true,\"Expression\":\"* * * * *\"}}}");

            _sut.AddScheduledService<BackgroundServiceStub>(config);
            Assert.AreEqual(2, _sut.Count);
        }

        [Test]
        public void AddScheduledService_MissingConfigSection_ThrowsException()
        {
            var config = new Mock<IConfiguration>();

            Assert.Throws<ScheduledServiceRegistrationException>(() => _sut.AddScheduledService<BackgroundServiceStub>(config.Object),
                "Missing scheduled service configuration. Verify a section with name ScheduledServices:BackgroundServiceStub exists");
        }

        public class BackgroundServiceStub : ScheduledServiceBase
        {
            public BackgroundServiceStub(string expression) : base(expression)
            {
            }

            public BackgroundServiceStub(string expression, ILogger logger) : base(expression, logger)
            {
            }

            public override Task ExecuteJobAsync(CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
