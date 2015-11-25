using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBusinessObjectReferenceDataSourceControlValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBusinessObjectReferenceDataSourceControlValidatorFactory (
              new IBusinessObjectReferenceDataSourceControlValidatorFactory[] { new BocReferenceDataSourceValidatorFactory() });
      var factory = new FilteringBusinessObjectReferenceDataSourceControlValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<BusinessObjectReferenceDataSourceControl>();

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocReferenceDataSourceValidator) }));
    }
  }
}