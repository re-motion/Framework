using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class IBusinessObjectReferenceDataSourceControlValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>>());

      var factories = ((CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>)instance).VlidatorFactories;
      Assert.That(
          factories.Select(f => f.GetType()),
          Is.EqualTo(new[] { typeof(ValidationBocReferenceDataSourceValidatorFactory) }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
