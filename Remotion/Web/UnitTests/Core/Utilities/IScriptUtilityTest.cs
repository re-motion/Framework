using System;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class IScriptUtilityTest
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
      var factory = _serviceLocator.GetInstance<IScriptUtility>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(ScriptUtility)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IScriptUtility>();
      var factory2 = _serviceLocator.GetInstance<IScriptUtility>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
