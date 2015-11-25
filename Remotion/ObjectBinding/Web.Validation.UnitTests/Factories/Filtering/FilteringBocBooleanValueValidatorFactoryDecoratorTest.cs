using System;
using System.Linq;
using System.Web.UI.WebControls;
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
  public class FilteringBocBooleanValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocBooleanValueValidatorFactory (
              new IBocBooleanValueValidatorFactory[] { new BocBooleanValueValidatorFactory(), new BocValidatorFactory() });
      var factory = new FilteringBocBooleanValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocBooleanValue>();
      control.Expect (c => c.IsRequired).Return (true);
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocValidator), typeof(CompareValidator) }));
    }
  }
}