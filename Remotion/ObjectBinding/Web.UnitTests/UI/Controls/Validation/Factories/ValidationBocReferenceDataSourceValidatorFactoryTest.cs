using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation.Factories
{
  [TestFixture]
  public class ValidationBocReferenceDataSourceValidatorFactoryTest
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

      var factories =
          ((CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>)instance).VlidatorFactories.Select(f => f.GetType()).ToList();
      Assert.That(factories, Has.Member(typeof(ValidationBocReferenceDataSourceValidatorFactory)));
      Assert.That(factories.Count, Is.EqualTo(1));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocAutoCompleteReferenceValue (bool isReadOnly)
    {
      var mock = new Mock<BusinessObjectReferenceDataSourceControl>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBocReferenceDataSourceValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly).ToArray();

      Assert.That(
          validators.Select(v => v.GetType()),
          Is.EquivalentTo(new[] { typeof(BusinessObjectReferenceDataSourceControlValidationResultDispatchingValidator) }));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }
  }
}
