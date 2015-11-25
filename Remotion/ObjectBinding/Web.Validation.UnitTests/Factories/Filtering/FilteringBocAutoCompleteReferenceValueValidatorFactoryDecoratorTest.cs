using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocAutoCompleteReferenceValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocAutoCompleteReferenceValueValidatorFactory (
              new IBocAutoCompleteReferenceValueValidatorFactory[] { new BocAutoCompleteReferenceValueValidatorFactory(), new BocValidatorFactory() });
      var factory = new FilteringBocAutoCompleteReferenceValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocAutoCompleteReferenceValue>();
      control.Expect (c => c.IsRequired).Return (true);
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocValidator), typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }));
    }
  }
}