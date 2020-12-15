using System;
using System.Linq;
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation.Factories
{
  [TestFixture]
  public class ValidationBocReferenceDataSourceValidatorFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var instance = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>>());

      var factories =
          ((CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>) instance).VlidatorFactories.Select (f => f.GetType()).ToList();
      Assert.That (factories, Has.Member (typeof (ValidationBocReferenceDataSourceValidatorFactory)));
      Assert.That (factories.Count, Is.EqualTo (1));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocAutoCompleteReferenceValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<BusinessObjectReferenceDataSourceControl>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new ValidationBocReferenceDataSourceValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly).ToArray();

      if (isReadOnly)
      {
        Assert.That (validators, Is.Empty);
      }
      else
      {
        Assert.That (
            validators.Select (v => v.GetType()),
            Is.EquivalentTo (new[] { typeof (BusinessObjectReferenceDataSourceControlValidationResultDispatchingValidator) }));
        Assert.That (validators, Has.All.Property ("EnableViewState").False);
      }
    }
  }
}