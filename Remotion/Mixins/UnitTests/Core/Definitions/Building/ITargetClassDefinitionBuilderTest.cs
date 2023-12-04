using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions.Building;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class ITargetClassDefinitionBuilderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<ITargetClassDefinitionBuilder>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(TargetClassDefinitionBuilder)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<ITargetClassDefinitionBuilder>();
      var factory2 = _serviceLocator.GetInstance<ITargetClassDefinitionBuilder>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
