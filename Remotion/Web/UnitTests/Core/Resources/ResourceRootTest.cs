using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourceRootTest
  {
    [Test]
    public void GetInstance_Once ()
    {
      var serviceLocator = DefaultServiceLocator.Create();

      var factory = serviceLocator.GetInstance<ResourceRoot>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(ResourceRoot)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var serviceLocator = DefaultServiceLocator.Create();

      var factory1 = serviceLocator.GetInstance<ResourceRoot>();
      var factory2 = serviceLocator.GetInstance<ResourceRoot>();

      Assert.That(factory1, Is.SameAs(factory2));
    }

    [Test]
    public void GetValue_ReturnsRes ()
    {
      var resourceRoot = new ResourceRoot();

      var value = resourceRoot.Value;

      Assert.That(value, Is.EqualTo("res"));
    }
  }
}
