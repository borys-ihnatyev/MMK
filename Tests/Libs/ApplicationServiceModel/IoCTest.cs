using MMK.ApplicationServiceModel.Locator;
using MMK.ApplicationServiceModel.Locator.Resolving;
using NUnit.Framework;

namespace MMK.ApplicationServiceModel
{
    [TestFixture]
    public class IoCTest
    {
        [Test]
        [ExpectedException(typeof(ServiceLocatorNotFoundException))]
        public void ThrowsServiceLocatorNotFoundException()
        {
            IoC.ServiceLocator.Get<object>();
        }
    }
}