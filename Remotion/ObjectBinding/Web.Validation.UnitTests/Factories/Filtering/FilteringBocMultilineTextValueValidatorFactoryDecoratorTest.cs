using System;
using System.Linq;
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocMultilineTextValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocMultilineTextValueValidatorFactory (
              new IBocMultilineTextValueValidatorFactory[] { new BocMultilineTextValueValidatorFactory(), new BocValidatorFactory() });
      var factory = new FilteringBocMultilineTextValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocMultilineTextValue>();
      control.Expect (c => c.IsRequired).Return (true);
      control.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle() { MaxLength = 10 });
      control.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocValidator) }));
    }
  }
}