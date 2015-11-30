using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocCheckBoxValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocCheckBoxValidatorFactory (
              new IBocCheckBoxValidatorFactory[] { new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory() });
      var factory = new FilteringBocCheckBoxValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocCheckBox>();
      control.Expect (c => c.IsRequired).Return (true);
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BusinessObjectBoundEditableWebControlValidator) }));
    }
  }
}