using System;
using System.Linq;
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocEnumValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocEnumValueValidatorFactory (
              new IBocEnumValueValidatorFactory[] { new BocEnumValueValidatorFactory(), new BocValidatorFactory() });
      var factory = new FilteringBocEnumValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocEnumValue>();
      control.Expect (c => c.IsRequired).Return (true);
      control.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocValidator) }));
    }
  }
}