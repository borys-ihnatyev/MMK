using MMK.Application;
using Moq;
using NUnit.Framework;

namespace MMK.ApplicationServiceModel
{
    [TestFixture]
    public class ServiceContainerTest
    {
        private ServiceContainer serviceContainer;

        [SetUp]
        public void SetUp()
        {
            serviceContainer = new ServiceContainer();
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void AddService()
        {
            var dummyService = Mock.Of<IService>();
            serviceContainer.AddService(dummyService);
        }

        [Test]
        public void ShoudGetServiceAfterAdd()
        {
            var dummyService = Mock.Of<IService>();
            serviceContainer.AddService(dummyService);

            Assert.IsNotNull(serviceContainer.GetService<IService>());
        }
    }
}