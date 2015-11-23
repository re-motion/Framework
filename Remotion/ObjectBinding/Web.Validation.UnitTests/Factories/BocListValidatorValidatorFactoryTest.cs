using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories
{
  [TestFixture]
  public class BocListValidatorValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocListValidatorFactory>();

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());

      var factories = ((CompoundValidatorFactory<IBocList>) instance).VlidatorFactories.Select (f => f.GetType()).ToList();
      Assert.That (factories, Has.Member (typeof (BocListValidatorFactory)));
      Assert.That (factories, Has.Member (typeof (BocListValidatorValidatorFactory)));
      Assert.That (factories.IndexOf (typeof (BocListValidatorFactory)), Is.LessThan (factories.IndexOf (typeof (BocListValidatorValidatorFactory))));
      Assert.That (factories.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocListValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocListValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocAutoCompleteReferenceValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocList>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new BocListValidatorValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);

      if (isReadOnly)
        Assert.That (validators, Is.Empty);
      else
        Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (new[] { typeof (BocListValidator) }));
    }
  }
}