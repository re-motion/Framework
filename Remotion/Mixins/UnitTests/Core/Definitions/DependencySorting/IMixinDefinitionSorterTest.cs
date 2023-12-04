using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting
{
  [TestFixture]
  public class IMixinDefinitionSorterTest
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
      var factory = _serviceLocator.GetInstance<IMixinDefinitionSorter>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(MixinDefinitionSorter)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IMixinDefinitionSorter>();
      var factory2 = _serviceLocator.GetInstance<IMixinDefinitionSorter>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
